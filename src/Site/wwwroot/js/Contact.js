function showContactDialog()
{
	// reset controls
	$('#txtContactMessage').val('');
	$('#vldContact').hide();

	// show modal form
	$('#dlgContact').modal();
}

function sendContact()
{
	// validation
	let message = $('#txtContactMessage').val();

	if (!message)
	{
		$('#vldContact').show();
	}
	else
	{
		var fd = new FormData();
		fd.append('message', message);

		$.ajax(
			{
				url: `/Home/SendMessage`,
				method: 'POST',
				processData: false,
				contentType: false,
				data: fd,
				error: (ex) =>
				{
					throw ex;
				}
			});

		// close immidiately
		$('#dlgContact').modal('hide');
	}
}