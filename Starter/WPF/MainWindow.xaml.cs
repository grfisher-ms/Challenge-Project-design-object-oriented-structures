using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using UI.ImageRendering;
using LangtonsAnt;
using Json;

namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IGame game;
        DispatcherTimer gameTimer;
        PlayUIMode playUiState;
        string rule = "LR";
        const int imagesPerSecond = 25;

        public MainWindow()
        {
            InitializeComponent();
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(1000 / imagesPerSecond);
            gameTimer.Tick += (sender, args) =>
            {
                game.NextGeneration();
                UpdateGameView(game!);
            };
            PlayUIState = PlayUIMode.Stopped;
        }

        private IGame CreateGame(string initialRule = "LR")
        {
            IGame newGame = null;
            newGame = new Game(128, null);
            newGame.Ants = new IAnt[] {
              new GeneralizedAnt(i: newGame.Size / 2 + 1, j: newGame.Size / 2 + 1, direction: 0) { Rule = initialRule }
            };
            return newGame;
        }


        #region Properties

        PlayUIMode PlayUIState
        {
            get { return playUiState; }
            set
            {
                switch (value)
                {
                    case PlayUIMode.Playing:
                        gameTimer.Start();
                        btnPlay.Visibility = Visibility.Collapsed;
                        btnPause.Visibility = Visibility.Visible;
                        break;
                    case PlayUIMode.Paused:
                        gameTimer.Stop();
                        btnPause.Visibility = Visibility.Collapsed;
                        btnPlay.Visibility = Visibility.Visible;
                        break;
                    case PlayUIMode.Stopped:
                        gameTimer.Stop();
                        game = CreateGame(rule);
                        UpdateGameView(game!);
                        btnPlay.Visibility = Visibility.Visible;
                        btnPause.Visibility = Visibility.Collapsed;
                        break;
                    default:
                        break;
                }
                playUiState = value;
            }
        }

        void SwithNonRuleButtonsEnabled(bool value)
        {
            // Play - Stop - Pause
            btnPlay.IsEnabled = value;
            btnStop.IsEnabled = value;
            btnPause.IsEnabled = value;

            // Save and load
            btnSave.IsEnabled = value;
            btnLoad.IsEnabled = value;
        }

        public int MaxColor
        {
            get { return (rule?.Length ?? 0) - 1; }
        }

        #endregion

        #region UI

        private void UpdateGameView(IGame gameState)
        {
            ImageSource source;
            source = GameImageRenderer.GetGenerationImageSourceX2(gameState);
            imgGame.Source = source;

            lblGenerationN.Text = gameState.GenerationN.ToString();
        }

        private List<TextBlock> CreateColoredRuleControls(string rule)
        {
            BrushConverter bc = new BrushConverter();
            return rule
                // Calculate colors for rule letters.
                .Select((c, index) => new { Char = c, Color = ColorBytes.ColorSequence[index] })
                .Select(cc => new TextBlock()
                {
                    Text = cc.Char.ToString(),
                    // Invert black text on very dark backgound.
                    Foreground = (cc.Color[0] + cc.Color[1] + cc.Color[2] < 128 * 2.5) ? Brushes.White : Brushes.Black,
                    // cc.Color is BGR
                    Background = new SolidColorBrush(Color.FromRgb(cc.Color[2], cc.Color[1], cc.Color[0])),
                    FontWeight = FontWeights.Bold
                }).ToList();
        }

        #endregion

        #region Event Handlers

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            PlayUIState = PlayUIMode.Playing;
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            PlayUIState = PlayUIMode.Paused;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            PlayUIState = PlayUIMode.Stopped;
        }

        private void btnStepBackward_Click(object sender, RoutedEventArgs e)
        {
            // TODO #4 Implement moving to the next game state here
            MessageBox.Show("Stepping back and forth is not implemented yet.");
        }

        private void btnStepForward_Click(object sender, RoutedEventArgs e)
        {
            // TODO #4 Implement moving to the next game state here
            MessageBox.Show("Stepping back and forth is not implemented yet.");
        }
        
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (game == null)
                throw new InvalidOperationException("Cannot save the game when current game state is null");

            PlayUIState = PlayUIMode.Paused;

            var saveFileDialog = new SaveFileDialog() { Filter = "JSON Document(*.json)|*.json" };
            if (saveFileDialog.ShowDialog() == true)
            {
                string jsonString = null;
                jsonString = GameJSONSerializer.ToJson(game);
                File.WriteAllText(saveFileDialog.FileName, jsonString);
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            PlayUIState = PlayUIMode.Paused;

            string fileName;
            var openFileDialog = new OpenFileDialog() { Filter = "JSON Document(*.json)|*.json" };
            if (openFileDialog.ShowDialog() == true)
            {
                fileName = openFileDialog.FileName;
                string json = File.ReadAllText(fileName);
                game = GameJSONSerializer.FromJson(json);
                UpdateGameView(game);
            }
        }


        #endregion
    }
}