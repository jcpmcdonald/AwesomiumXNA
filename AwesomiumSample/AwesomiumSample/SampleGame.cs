using System;
using System.IO;
using System.Runtime.InteropServices;
using Awesomium.Core;
using AwesomiumXNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AwesomiumSample
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class SampleGame : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		private Texture2D cursorTexture;
		private MouseState mouseState;
		private Random rand = new Random();

		private Texture2D stanTexture;
		Vector2 stanPosition = new Vector2(600, 300);

		// This represents a global JS object that will exist on all Awesomium pages we visit
		JSObject xnaObj;

		private AwesomiumComponent awesomium;

		public SampleGame()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// Allow the frame rate to go wild
			IsFixedTimeStep = false;
			graphics.SynchronizeWithVerticalRetrace = false;

			graphics.PreferredBackBufferWidth = 1920;
			graphics.PreferredBackBufferHeight = 1080;
			graphics.ApplyChanges();

			awesomium = new AwesomiumComponent(this, GraphicsDevice.Viewport.Bounds);
			awesomium.WebView.ParentWindow = Window.Handle;

			// Don't forget to add the awesomium component to the game
			Components.Add(awesomium);

			// Add a data source that will simply act as a pass-through
			awesomium.WebView.WebSession.AddDataSource("sample", new SampleDataSource());

			// This will trap all console messages
			awesomium.WebView.ConsoleMessage += WebView_ConsoleMessage;
			

			// A document must be loaded in order for me to make a global JS object, but the presence of
			// the global JS object affects the first page to be loaded, so give it an egg:
			awesomium.WebView.LoadHTML("<html><head><title>Loading...</title></head><body></body></html>");
			while(!awesomium.WebView.IsDocumentReady)
			{
				WebCore.Update();
			}

			// Trap log commands so that we can differentiate between log statements and JS errors
			JSObject console = awesomium.WebView.CreateGlobalJavascriptObject("console");
			console.Bind("log", false, WebView_ConsoleLog);
			console.Bind("dir", false, WebView_ConsoleLog);

			// Create an object that will allow JS inside Awesomium to communicate with XNA
			xnaObj = awesomium.WebView.CreateGlobalJavascriptObject("xna");
			xnaObj.Bind("exit", false, OnExit);

			// Some stuff I wrote for mouse handling on the HUD vs the Game
			xnaObj.Bind("OnMouseUp", false, OnMouseUp);
			xnaObj.Bind("OnMouseDown", false, OnMouseDown);

			xnaObj.Bind("btnUpPressed", false, btnUpPressed);
			xnaObj.Bind("btnDownPressed", false, btnDownPressed);
			xnaObj.Bind("btnLeftPressed", false, btnLeftPressed);
			xnaObj.Bind("btnRightPressed", false, btnRightPressed);

			// Load some content
			// NOTE: Awesomium wants us to do it this way
			awesomium.WebView.Source = @"asset://sample/index.html".ToUri();
			// NOTE: But this also works
			//awesomium.WebView.Source = new Uri(Environment.CurrentDirectory + @"\..\..\..\html\index.html");

			// Otherwise, you can just load web content like this:
			//awesomium.WebView.Source = @"http://google.com".ToUri();

			base.Initialize();
		}


		private void WebView_ConsoleLog(object sender, JavascriptMethodEventArgs javascriptMethodEventArgs)
		{
			Console.WriteLine(javascriptMethodEventArgs.Arguments[0].ToString());
		}


		void WebView_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
		{
			// All JS errors will come here
			throw new Exception(String.Format("Awesomium JS Error: {0}, {1} on line {2}", e.Message, e.Source, e.LineNumber));
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			cursorTexture = Content.Load<Texture2D>("Cursor");
			stanTexture = Content.Load<Texture2D>("Stan");

			base.LoadContent();
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
		}


		/// <summary>
		/// On Exit callback from JavaScript from Awesomium
		/// </summary>
		public void OnExit(object sender, JavascriptMethodEventArgs e)
		{
			Exit();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			mouseState = Mouse.GetState();

			// Send some test data to Awesomium
			if(awesomium.WebView.IsDocumentReady && !awesomium.WebView.IsLoading)
			{
				awesomium.WebView.ExecuteJavascript(String.Format("handleDataFromXNA(\"{0}\")", "Hello World: " + rand.Next(10, 99)));
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			base.Draw(gameTime);
			spriteBatch.Begin();

			spriteBatch.Draw(stanTexture, stanPosition, Color.White);

			// Awesomium should be drawn after the game has been drawn, assuming it's acting as a HUD
			if (awesomium.WebViewTexture != null)
			{
				spriteBatch.Draw(awesomium.WebViewTexture, GraphicsDevice.Viewport.Bounds, Color.White);
			}

			// And the mouse cursor should always be drawn last
			spriteBatch.Draw(cursorTexture, new Vector2(mouseState.X, mouseState.Y), Color.White);

			spriteBatch.End();
		}


		/// <summary>
		/// Handle mouse down everywhere except the controls
		/// </summary>
		protected void OnMouseDown(object sender, JavascriptMethodEventArgs e)
		{
			bool mouseDownOverHUD = e.Arguments[0];
			MouseButton mouseButton = (MouseButton)(int)e.Arguments[1];

		}


		/// <summary>
		/// Handle mouse up everywhere except the controls
		/// </summary>
		protected void OnMouseUp(object sender, JavascriptMethodEventArgs e)
		{
			bool mouseUpOverHUD = e.Arguments[0];
			MouseButton mouseButton = (MouseButton)(int)e.Arguments[1];
			bool clickHandled = mouseUpOverHUD;

			Rectangle stan = new Rectangle((int)stanPosition.X, (int)stanPosition.Y, stanTexture.Width, stanTexture.Height);
			if (stan.Contains(mouseState.X, mouseState.Y))
			{
				Console.WriteLine(mouseButton.ToString() + " clicked on Stan, and it was " + (clickHandled ? "" : "not ") + "handled by the Web UI");
			}
		}


		protected void btnUpPressed(object sender, JavascriptMethodEventArgs e)
		{
			stanPosition.Y -= 50;
		}

		protected void btnDownPressed(object sender, JavascriptMethodEventArgs e)
		{
			stanPosition.Y += 50;
		}

		protected void btnLeftPressed(object sender, JavascriptMethodEventArgs e)
		{
			stanPosition.X -= 50;
		}


		protected void btnRightPressed(object sender, JavascriptMethodEventArgs e)
		{
			stanPosition.X += 50;
		}
	}
}
