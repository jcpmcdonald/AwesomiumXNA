using System;
using System.Collections.Generic;
using System.Linq;
using Awesomium.Core;
using AwesomiumXNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TestSample
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class SampleGame : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		private Texture2D cursorTexture;
		private MouseState mouseState;
		private Random rand = new Random();

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
			// Allow the framerate to go wild
			IsFixedTimeStep = false;
			graphics.SynchronizeWithVerticalRetrace = false;

			graphics.PreferredBackBufferWidth = 1920;
			graphics.PreferredBackBufferHeight = 1080;
			graphics.ApplyChanges();

			awesomium = new AwesomiumComponent(this, GraphicsDevice.Viewport.Bounds);

			// Create an object that will allow JS inside Awesomium to communicate with XNA
			awesomium.WebView.CreateObject("xna");
			awesomium.WebView.SetObjectCallback("xna", "exit", OnExit);

			// Don't forget to add the awesomium component to the game
			Components.Add(awesomium);

			// Set the base directory if you plan to load local content
			WebCore.BaseDirectory = Environment.CurrentDirectory + @"\..\..\..\html\";
			awesomium.WebView.LoadFile(@"index.html");

			// Otherwise, you can just load web content like this:
			//awesomium.WebView.LoadURL(@"http://google.com");

			// If you don't give awesomium focus, most everything will work, but the cursor will not appear
			awesomium.WebView.Focus();

			base.Initialize();
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
		public void OnExit(object sender, JSCallbackEventArgs e)
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
			JSObject testData = new JSObject();
			testData["someString"] = new JSValue("Hello World");
			testData["someInt"] = new JSValue(rand.Next(10, 99));
			awesomium.WebView.CallJavascriptFunction("", "handleDataFromXNA", new JSValue(testData));

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

			// Awesomium should be drawn after the game has been drawn, assuming it's acting as a HUD
			if (awesomium.WebViewTexture != null)
			{
				spriteBatch.Draw(awesomium.WebViewTexture, GraphicsDevice.Viewport.Bounds, Color.White);
			}

			// And the mouse cursor should always be drawn last
			spriteBatch.Draw(cursorTexture, new Vector2(mouseState.X, mouseState.Y), Color.White);

			spriteBatch.End();
		}
	}
}
