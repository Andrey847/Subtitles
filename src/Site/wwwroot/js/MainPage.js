﻿var _currentWordIcon;

$(document).ready(function ()
{
	loadWords();

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

function loadWords()
{
	let selectedMovie = $('#cmbMovie option:selected').val();
	
	$.ajax({
		type: "GET",
		url: `/WorkPlace/AllWords/${selectedMovie}`,
		success: function (message)
		{
			generateTable(message);
		},
		error: function ()
		{
			alert("There was error uploading files");
		}
	});
}

function uploadSrt()
{
	var files = document.getElementById('file-upload').files;
	var data = new FormData();
	for (var i = 0; i < files.length; i++)
	{
		data.append(files[i].name, files[i]);
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

			loadWords();
		},
		error: function ()
		{
			alert("There was error uploading files");
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
									<div class="col-sm-1">${item.frequency}</div>
									<div class="col-sm-3" class="srt-english">
										<div class="srt-play-btn" onclick="playWord(this, '${item.source}');"></div>
										<span class="srt-text">${item.source}</span>
										<img class="srt-phrases-ico" onclick="showPhrases(this, '${item.id}')">
									</div>
									<div class="col-sm-3">
										<input type="text" class='form-control' value="${item.translation}"></input>
									</div>
									<div class="col-sm-2" onmouseenter="selectRow(this);" onmouseleave="deselectRow(this);">
										<button onclick="save('${item.source}', this);">Save</button>
										<button onclick="markLearned('${item.source}', this, ${item.id});">Learned</button>
									</div>
								</div>`);
	});
}

function selectRow(sender)
{
	$(sender).closest('.row').addClass('srt-selected-row');
}

function deselectRow(sender)
{
	$(sender).closest('.row').removeClass('srt-selected-row');
}

function showPhrases(sender, wordId)
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

		// request for phrases
		$.ajax({
			type: 'GET',
			url: `/WorkPlace/GetPhrases/${wordId}`,
			success: (phrases) =>
			{
				let phraseHtml = ''

				phrases.forEach((item) =>
				{
					phraseHtml += `<div class="srt-phrase">${item.value}</div>`;
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

function playWord(sender, word)
{
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
	let translation = $(sender).closest('.row').find('input').val();

	// simple async save
	$.ajax({
		type: 'POST',
		url: '/WorkPlace/SaveTranslation',
		contentType: 'application/json',
		data: JSON.stringify(
			{
				"source": source,
				"translation": translation
			})
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

	// simple async save
	$.ajax({
		type: 'POST',
		url: '/WorkPlace/MarkLearned',
		contentType: 'application/json',
		data: JSON.stringify(
			{
				"source": source
			}),
		success: function ()
		{
			$(sender).closest('.row').remove();
		}
	});
}

function deleteMovie()
{
	if (confirm("Are you sure? All data, linked to this movie, will be removed"))
	{
		let selectedMovie = $('#cmbMovie option:selected').val();

		$.ajax({
			type: "DELETE",
			url: `/WorkPlace/DeleteMovie/${selectedMovie}`,
			success: function ()
			{
				loadWords();
			},
			error: function ()
			{
				// redirect to the main page.
				window.location.href = '/';
			}
		});
	}
}