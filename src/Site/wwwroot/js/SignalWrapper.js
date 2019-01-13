/**
 * Simple wrapper under signalR but with avility to reconnect in case of error.
 * */
class SignalWrapper
{
	/**
	 * Main constructor.
	 * @param {string} hub ( for example, /NotificationHub)
	 * @param {any} actions this is an array that contains object {method, action}. Where:
	 * method is signalR method(event). For example UploadProgress.
	 * action(parameter) is function to be executed. Possible input parameter is data from SignalR.
	 */
	constructor(hub, actions)
	{
		this.hub = hub;
		this.actions = actions;

		this.init();
	}

	init()
	{
		var that = this;

		this.isFirstConnection = true;
		this.connection = new signalR.HubConnectionBuilder()
			.withUrl(this.hub)
			.build();

		this.actions.forEach((a) =>
		{
			this.connection.on(a.method, (p) =>
			{
				console.log('Server notification. Reloading data...');
				a.action(p);
			});
		})

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
					this.actions.forEach((a) => a.action());
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


