/**
 * Simple wrapper under signalR but with avility to reconnect in case of error.
 * */
class SignalWrapper
{
	/**
	 * Main constructor.
	 * @param {string} hub
	 * @param {string} method
	 * @param {method} action - function to be executed. Possible input parameter is data from SignalR.
	 */
	constructor(hub, method, action)
	{
		this.hub = hub;
		this.method = method;
		this.action = action;
	}

	init()
	{
		var that = this;

		this.isFirstConnection = true;
		this.connection = new signalR.HubConnectionBuilder()
			.withUrl(this.hub)
			.build();

		this.connection.on(settings.method, () =>
		{
			console.log('Server notification. Reloading data...');
			settings.action();
		});

		this.connection.start()
			.then(() =>
			{
				if (that.isFirstConnection)
				{
					that.isFirstConnection = false;
				}
				else
				{
					// to reinitialize screen and controller after disconnection
					settings.action();
				}
			})
			.catch(err =>
			{
				console.error(err.toString());

				setTimeout(() =>
				{
					console.log("Reconnecting...");
					that.init();
				}, 5000);
			});

		this.connection.onclose(function ()
		{
			setTimeout(() =>
			{
				console.log("Reconnecting...");
				that.init();
			}, 5000);
		});
	}


}


