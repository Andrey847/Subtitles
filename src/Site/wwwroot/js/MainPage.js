var _currentWordIcon;

$(document).ready(function ()
{
	$.ajax({
		type: "GET",
		url: "/WorkPlace/AllWords",
		success: function (message)
		{
			generateTable(message);
		},
		error: function ()
		{
			alert("There was error uploading files");
		}
	});

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
			generateTable(message);
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

	jsonWords.forEach((item) =>
	{
		container.append(`<div class="row">
									<div class="col-sm-1">${item.frequency}</div>
									<div class="col-sm-3" class="srt-english">
										<div class="srt-play-btn" onclick="playWord(this, '${item.english}');"></div>
										<span class="srt-text">${item.english}</span>
										<img class='srt-phrases-ico'>
									</div>
									<div class="col-sm-3">
										<input type="text" class='form-control' value="${item.translation}"></input>
									</div>
									<div class="col-sm-2" onmouseenter="selectRow(this);" onmouseleave="deselectRow(this);">
										<button onclick="save('${item.english}', this);">Save</button>
										<button onclick="markLearned('${item.english}', this);">Learned</button>
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



function save(english, sender)
{
	let translation = $(sender).closest('.row').find('input').val();

	// simple async save
	$.ajax({
		type: 'POST',
		url: '/WorkPlace/SaveTranslation',
		contentType: 'application/json',
		data: JSON.stringify(
			{
				"english": english,
				"translation": translation
			})
	});
}

function markLearned(english, sender)
{
	// simple async save
	$.ajax({
		type: 'POST',
		url: '/WorkPlace/MarkLearned',
		contentType: 'application/json',
		data: JSON.stringify(
			{
				"english": english
			}),
		success: function ()
		{
			$(sender).closest('.row').remove();
		}
	});
}