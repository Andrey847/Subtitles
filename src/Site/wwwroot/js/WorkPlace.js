
const connection = new signalR.HubConnectionBuilder()
	.withUrl("/NotificationHub")
	.build();




connection.on("UploadProgress", (percentCompleted) =>
{
	
	initializeNotifications()
	if (percentCompleted == 100)
	{
		// Everything completed. Reload movies and then Remove %  and unblock Upload button
		reloadMovies();
	}
	else
	{
		$('#btnUpload').text(`Upload ${percentCompleted}%`);

		// this waiter is shown until first percent changes.
		$('#imgUploadWaiter').hide();
	}
});

function signalConnect(cnt)
{
	console.log('SignalR: connecting');
	cnt.start().catch(err =>
	{
		console.error(err.toString());

		// reconnect after 5 seconds.
		setTimeout(() => signalConnect(cnt), 5000);

		c += 1;
	});
}

var _currentWordIcon;
var _loadingFile;
var _undoQueue = [];

$(document).ready(function ()
{
	signalConnect(connection);

	restoreLayout();

	var wordPlayer = document.getElementById('wordPlayer');
	wordPlayer.onloadstart = function ()
	{
		$(_currentWordIcon).removeClass('srt-play-btn').addClass('srt-wait-btn');
	};

	wordPlayer.onabort = function ()
	{
		$(_currentWordIcon).addClass('srt-play-btn').removeClass('srt-wait-btn');
	};

	wordPlayer.onloadeddata = function ()
	{
		$(_currentWordIcon).addClass('srt-play-btn').removeClass('srt-wait-btn');
		wordPlayer.play();
	};
});

// Reloads all movies of combobox.
function reloadMovies()
{
	$('#cmbMovie').empty();

	$.ajax(
		{
			type: "GET",
			url: `/WorkPlace/GetMovies/`,
			success: (movies) =>
			{
				let cmbMovie = $('#cmbMovie');

				cmbMovie.append(`<option value="0">[All]</option>`);

				movies.forEach((item) =>
				{
					let selected = item.name == _loadingFile ? ' selected ' : '';

					// mark archived movies as italic
					let content = item.isArchived ? `[Archived] ${item.name}` : item.name;
					cmbMovie.append(`<option value="${item.id}" data-val-isArchived="${item.isArchived}" ${selected}>${content}</option>`);
				});

				loadWords();

				$('#btnUpload').text('Upload');
				$('#btnUpload').attr('disabled', null);
			}
		});
}

function loadWords()
{
	// immidiately clear undo queue as it used hidden elements that will be deleted by this method.
	clearUndo();

	let selectedMovie = $('#cmbMovie option:selected').val();
	$('#btnRefresh').removeClass('btn-waiter-hidden');

	$.ajax({
		type: "GET",
		url: `/WorkPlace/AllWords/${selectedMovie}`,
		success: function (message)
		{
			generateTable(message);
		},
		error: function (ex)
		{
			throw ex;
		},
		complete: () =>
		{
			$('#btnRefresh').addClass('btn-waiter-hidden');
		}
	});
}

function uploadSrt(files)
{
	// disable the upload button during the load.
	$('#btnUpload').attr('disabled', 'disabled');
	$('#imgUploadWaiter').show();

	var data = new FormData();
	for (var i = 0; i < files.length; i++)
	{
		data.append(files[i].name, files[i]);

		// doesn't matter what is the files from the list. In most cases there is one file only.
		// and we show it as selected after the loading.
		_loadingFile = files[i].name;
	}

	$.ajax({
		type: "POST",
		url: "/WorkPlace/UploadSrt",
		contentType: false,
		processData: false,
		data: data,
		success: function (message)
		{
			alert(`File imported. Total word: ${message.totalWords},
new words: ${message.newWords}`);
		},
		error: function ()
		{
			alert("There was error uploading files");
			$('#imgUploadWaiter').hide();
		}
	});
}

function generateTable(jsonWords)
{
	let container = $("#dvWords");
	container.empty();

	jsonWords.forEach((item) =>
	{
		container.append(`<div class="row">
									<div class="col-sm-1"><span class="srt-freq">${item.frequency}</span></div>
									<div class="col-sm-3 srt-english">
										<div class="srt-play-btn" onclick="playWord(this, ${item.id}, '${item.source}');"></div>
										<span class="srt-text">${item.source}</span>
										<img class="srt-phrases-ico" onclick="showPhrases(this, '${item.id}', '${item.source}')">
									</div>
									<div class="col-sm-3">
										<input type="text" class='form-control srt-txt srt-txt-normal' value="${item.translation}" onkeyup="wordSubmit(event, '${item.source}', this);"></input>
									</div>
									<div class="col-sm-2" onmouseenter="selectRow(this);" onmouseleave="deselectRow(this);">
										<button class="btn btn-sm btn-secondary" onclick="save('${item.source}', this);">Save</button>
										<button class="btn btn-sm btn-secondary srt-btn-learned" onclick="markLearned('${item.source}', this, ${item.id});">Learned</button>
									</div>
								</div>`);
	});
}

// Blinks control if everything is ok.
function successBlink(jControl)
{
	jControl.removeClass('srt-txt-normal');

	// it is impossible just remove and add class for blinking effect. small delay is required.
	setTimeout(() =>
	{
		jControl.addClass('srt-txt-normal');
	},
		10)
}

function wordSubmit(event, source, sender)
{
	if (event.keyCode === 13)
	{
		event.preventDefault();
		save(source, sender);
	}
}

function selectRow(sender)
{
	$(sender).closest('.row').addClass('srt-selected-row');
}

function deselectRow(sender)
{
	$(sender).closest('.row').removeClass('srt-selected-row');
}

function showPhrases(sender, wordId, word)
{
	let existingContainer = $('.srt-phrase-container');

	if (existingContainer.length > 0
		&& existingContainer.attr('data-val-wordid') == wordId)
	{
		// user clicks on the same button - just hide it
		existingContainer.remove();
	}
	else
	{
		// remove all existing phrases
		existingContainer.remove();

		// find parent row. we expand phrases after it
		let parentRow = $(sender).closest('.row');
		let selectedMovie = $('#cmbMovie option:selected').val();
		word = word.toLowerCase(); // for correct comparing.

		// request for phrases
		$.ajax({
			type: 'GET',
			url: `/WorkPlace/GetPhrases/${wordId}/${selectedMovie}`,
			success: (phrases) =>
			{
				let phraseHtml = ''

				phrases.forEach((item) =>
				{
					let phrase = '';

					item.value.split(' ').forEach((w) =>
					{
						if (w) // do not add empty spans.
						{
							let toCompare = w.toLowerCase()
								.replace(/[^\w\s-]|_/g, "")
								.replace(/\s+/g, " ");

							phrase += (toCompare == word)
								? `<span class='srt-searched-word'>${w}</span>`
								: `<span>${w}</span>`;
						}
					});
					phraseHtml += `<div class="srt-phrase">${phrase}</div>`;
				});

				// add element
				parentRow.after(`
				<div class="row srt-phrase-container" data-val-wordid="${wordId}">
					<div class="col-sm-2"></div>
					<div class="col-sm-10">
						${phraseHtml}
					</div>
				</div>`
				);

				// very strange but transition work with a tiny delay only.
				setTimeout(() => $('.srt-phrase-container')[0].style.maxHeight = '500px', 1);

			}
		});
	}
}

function playWord(sender, wordId, word)
{
	let existingContainer = $('.srt-phrase-container');

	if (existingContainer.length > 0
		&& existingContainer.attr('data-val-wordid') != wordId)
	{
		// close shown phrases if they are of another word.
		existingContainer.remove();
	}

	_currentWordIcon = sender;

	var player = document.getElementById('wordPlayer');

	if (player.src)
	{
		player.pause(); // stops previous playing.
		player.currentTime = 0;
	}

	player.src = `/WorkPlace/WordSound/${encodeURIComponent(word)}`;
	player.load();
}

function save(source, sender)
{
	let txtControl = $(sender).closest('.row').find('input');
	let translation = txtControl.val();

	// simple async save
	$.ajax({
		type: 'POST',
		url: '/WorkPlace/SaveTranslation',
		contentType: 'application/json',
		data: JSON.stringify(
			{
				"source": source,
				"translation": translation
			}),
		success: () => successBlink(txtControl)
	});
}

function markLearned(source, sender, wordId)
{
	// close phrases for the word, if we marked it as learned.
	let existingContainer = $('.srt-phrase-container');
	if (existingContainer.length > 0
		&& existingContainer.attr('data-val-wordid') == wordId)
	{
		existingContainer.remove();
	}

	// html that contains such word (to be shown in undo case in the future)
	let htmlContainer = $(sender).closest('.row');

	// simple async save
	$.ajax({
		type: 'POST',
		url: '/WorkPlace/MarkLearned',
		contentType: 'application/json',
		data: JSON.stringify(
			{
				"source": source
			}),
		success: () =>
		{
			// update the undo stack (add source, which is word);
			addToUndo(source, htmlContainer);
		}
	});

	// hide learned word immidiately, as saving is in the background.
	htmlContainer.hide();
}

// adds new item to undo
function addToUndo(word, container)
{
	_undoQueue.push(
		{
			word: word,
			container: container
		});

	$('#btnUndo').prop('disabled', null);
}

// Performs undo operation
function performUndo()
{
	let undoItem = _undoQueue.pop();

	if (undoItem != null)
	{
		undoItem.container.show();

		$.ajax({
			type: 'POST',
			url: '/WorkPlace/MarkUnlearned',
			contentType: 'application/json',
			data: JSON.stringify(
				{
					"source": undoItem.word
				})
		});
	}

	if (_undoQueue.length == 0)
	{
		// disable undo button as there are no other words to be undo.
		$('#btnUndo').prop('disabled', 'disabled');
	}
}

// clears undo queue. For example, after refresh.
function clearUndo()
{
	_undoQueue = [];
	$('#btnUndo').prop('disabled', 'disabled');
}

function deleteMovie()
{
	if (confirm("Are you sure? All data, linked to this movie, will be removed"))
	{
		let selectedMovie = $('#cmbMovie option:selected').val();

		$.ajax({
			type: "DELETE",
			url: `/WorkPlace/DeleteMovie/${selectedMovie}`,
			success: () =>
			{
				// select [All] movies
				$('#cmbMovie').val('0');
				loadWords();

				// do not forget to remove deleted movie.
				$(`#cmbMovie option[value='${selectedMovie}']`).remove();
			},
			error: (e) => { throw e; }
		});
	}
}

function setArchiveState(archive)
{
	let selectedMovie = $('#cmbMovie option:selected').val();

	$.ajax({
		type: "POST",
		url: `/WorkPlace/SetArchiveState/${selectedMovie}/${archive}`,
		success: () =>
		{
			reloadMovies();
			$('#dlgRenameMovie').modal('hide');
		},
		error: (e) => { throw e; }
	});
}

function renameMovieDialog()
{
	let movieId = $('#cmbMovie option:selected').val();
	let isArchived = $('#cmbMovie option:selected').attr('data-val-isArchived');

	if (movieId != '0') // 0 is [All]. Of course we cannot rename it.
	{
		$('#txtMovieName').val($('#cmbMovie option:selected').text());
		$('#vldMovieName').hide();

		// archive/unarchive buttons
		$('#btnArchiveMovie').hide();
		$('#btnUnArchiveMovie').hide();
		if (isArchived)
		{
			$('#btnUnArchiveMovie').show();
		}
		else
		{
			$('#btnArchiveMovie').show();
		}

		$('#dlgRenameMovie').modal();
	}
}

function renameMovie()
{
	// validation
	let newName = $('#txtMovieName').val();

	if (!newName)
	{
		$('#vldMovieName').text('Please enter name.');
		$('#vldMovieName').show();
	}
	else
	{
		$('#btnMovieRename').attr('disabled', 'disabled');
		$('#vldMovieName').hide();

		let movieId = $('#cmbMovie option:selected').val();

		$.ajax({
			type: "POST",
			url: `/WorkPlace/RenameMovie/${movieId}/${encodeURIComponent(newName)}`,
			success: (error) =>
			{
				if (error)
				{
					// unable to rename. Show this error
					$('#vldMovieName').text(error);
					$('#vldMovieName').show();
				}
				else
				{
					// everything is ok. close form 
					$('#dlgRenameMovie').modal('hide');
					$('#cmbMovie option:selected').text(newName);
				}
			},
			error: (e) => { throw e; },
			complete: () => $('#btnMovieRename').attr('disabled', null)
		});
	}
}

function fileChooseClick(evnt)
{
	var jButton = $(evnt.toElement);

	var fileBrowser = jButton.parent().find('input[type="file"]');

	// emulate click.
	fileBrowser.click();
}

function fileDragEnter(ev)
{
	ev.preventDefault();
	$('#dvUploadArea').addClass('drag-highlight');
}

function fileDragLeave(ev)
{
	$('#dvUploadArea').removeClass('drag-highlight');
	ev.stopPropagation();
}

function fileDropped(ev)
{
	ev.stopPropagation();

	fileDragLeave(event);
	// to prevent default browser behaviour (file to be opened).
	ev.preventDefault();

	uploadSrt(ev.dataTransfer.files)
}

function allowDrop(ev)
{
	ev.preventDefault();
}

function saveLayout()
{
	var layout =
	{
		selectedMovieId: $('#cmbMovie option:selected').val()
	}

	// simple async save
	$.ajax({
		type: 'POST',
		url: 'WorkPlace/SaveLayout',
		contentType: 'application/json',
		data: JSON.stringify(layout),
		error: (err) =>
		{
			throw err;
		}
	});
}

function restoreLayout()
{
	// restores layout
	$.ajax("WorkPlace/GetLayout",
		{
			async: false,
			method: 'GET',
			success: function (data)
			{
				var state = JSON.parse(data);

				if (state && state.selectedMovieId)
				{
					// Assign selected movie.
					$('#cmbMovie').val(state.selectedMovieId);

					loadWords();
				}
				else
				{
					// just show default selected value.
					loadWords();
				}
			},
			error: function (err)
			{
				throw err;
			}
		});
}