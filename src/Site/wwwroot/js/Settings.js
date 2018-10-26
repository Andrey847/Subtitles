var _settings;
var _cmbLanguage;
var _cmbUnknownWordMax;

$(document).ready(function ()
{
	_cmbLanguage = $('#cmbLanguage');
	_cmbUnknownWordMax = $('#cmbUnknownWordMax');

	// loads settings
	$.ajax(
		{
			url: `/Settings/GetSettings`,
			method: 'GET',
			success:
				(settings) =>
				{
					_settings = settings;

					_cmbLanguage.val(_settings.currentLanguageCode);
					_cmbUnknownWordMax.val(_settings.unknownWordMax);

					// Unblock controls
					blockControls(false);
				}
		});
});

function saveLanguage()
{
	_settings.currentLanguageCode = _cmbLanguage.val();
	saveSettings(() => successBlink(_cmbLanguage));	
}

function saveUnknownWordMax()
{
	_settings.unknownWordMax = _cmbUnknownWordMax.val();
	saveSettings(() => successBlink(_cmbUnknownWordMax));	
}

// Blinks control if everything is ok.
function successBlink(jControl)
{
	jControl.removeClass('srt-save-normal');

	// it is impossible just remove and add class for blinking effect. small delay is required.
	setTimeout(() =>
	{
		jControl.addClass('srt-save-normal');
	},
	10)
}

// saves settings.
// success is handler after success processing (in order to show animation);
function saveSettings(success)
{
	blockControls(true);

	$.ajax(
		{
			url: `/Settings/UpdateSettings`,
			contentType: 'application/json',
			data: JSON.stringify(_settings),
			method: 'POST',
			success: () =>
			{
				blockControls(false);
				success();
			},
			error: (er) => alert('Unable to save settings: ' + er)
		}
	);
}

function blockControls(block)
{
	let state = block ? 'disabled' : null;
	_cmbLanguage.attr('disabled', state);
	_cmbUnknownWordMax.attr('disabled', state);	
}