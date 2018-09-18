$(document).ready(function ()
{
	let showGreeting = $('#hfShowGreeting').val();

	if (showGreeting)
	{
		$('#dlgGreeting').modal();
	}
});

function redirectToLogin()
{
	window.location.href = `/Account/Login`;
}