using System;
using System.IO;
using System.Linq;
using System.Media; 
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hakan_GULESEN_Hangman
{
    public partial class MainWindow : Window
    {
        private string[] mots;
        private string motATrouver;
        private char[] motCache;
        private int erreurs;

        private MediaPlayer mediaPlayer = new MediaPlayer();
        private bool isPlaying = false;

        public MainWindow()
        {
            InitializeComponent();
            mediaPlayer.Open(new Uri(@"C:\Users\SIO\Downloads\Hakan GULESEN Hangman\Hakan GULESEN Hangman\hangman.mp3", UriKind.Absolute));
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;

            ChargerMots();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReinitialiserJeu();
        }

        private void ChargerMots()
        {
            string cheminFichier = "Ressources/mots.txt";
            string cheminComplet = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, cheminFichier);

            if (File.Exists(cheminComplet))
            {
                mots = File.ReadAllLines(cheminComplet);
            }
            else
            {
                MessageBox.Show($"Impossible de trouver le fichier dans : {cheminComplet}. assurez vous que le fichier existe et que le chemin est le bon");
                mots = Array.Empty<string>();
            }
        }

        private void ReinitialiserJeu()
        {
            if (mots.Length == 0)
            {
                MessageBox.Show("Il n'y a aucun mot disponible, assurez vous que le fichier mots.txt contient des mots");
                return;
            }

            erreurs = 0;
            motATrouver = mots[new Random().Next(mots.Length)].ToUpper();
            motCache = new string('_', motATrouver.Length).ToCharArray();
            motcache.Text = new string(motCache);

            var boutons = GetClavierButtons();
            foreach (var bouton in boutons)
            {
                bouton.IsEnabled = true;
                bouton.Background = Brushes.LightBlue;
                bouton.Foreground = Brushes.White;
            }

            ReinitialiserPendu();
        }

        private Button[] GetClavierButtons()
        {
            return new[] { BtnA, BtnB, BtnC, BtnD, BtnE, BtnF, BtnG, BtnH, BtnI,
                BtnJ, BtnK, BtnL, BtnM, BtnN, BtnO, BtnP, BtnQ, BtnR,
                BtnS, BtnT, BtnU, BtnV, BtnW, BtnX, BtnY, BtnZ };
        }

        private void TirerMot_Click(object sender, RoutedEventArgs e)
        {
            ReinitialiserJeu();
        }

        private void Lettre_Click(object sender, RoutedEventArgs e)
        {
            string soundFileName = "click.wav"; // Nom du fichier 
            SoundPlayer player = new SoundPlayer();
            player.SoundLocation = System.IO.Path.Combine(@"C:\Users\SIO\Downloads\Hakan GULESEN Hangman\Hakan GULESEN Hangman\Sounds", soundFileName);

            try
            {
                player.Load(); // Chargement son bouton
                player.Play(); // Play
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show($"Vérifiez qu'un fichier audio existe à l'emplacement spécifié : {player.SoundLocation}");
            }

            var bouton = sender as Button;
            if (bouton == null || bouton.IsEnabled == false) return;

            char lettre = bouton.Tag.ToString()[0];
            bouton.IsEnabled = false;

            // Griser boutons si juste/faux
            bouton.Background = Brushes.Gray;
            bouton.Foreground = Brushes.Black;

            if (motATrouver == null)
            {
                MessageBox.Show("Il n'y a pas encore de mot, tirez au sort !");
                return;
            }

            if (motATrouver.Contains(lettre))
            {
                for (int i = 0; i < motATrouver.Length; i++)
                {
                    if (motATrouver[i] == lettre)
                    {
                        motCache[i] = lettre;
                    }
                }

                motcache.Text = new string(motCache);

                if (!motCache.Contains('_'))
                {
                    MessageBox.Show("Vous avez gagné la partie");
                    ReinitialiserJeu();
                }
            }
            else
            {
                erreurs++;
                MettreAJourPendu();

                if (erreurs >= 6)
                {
                    MessageBox.Show($"Vous avez perdu, le mot correct était : {motATrouver}");
                    ReinitialiserJeu();
                }
            }
        }

        private void ReinitialiserPendu()
        {
            tete.Visibility = Visibility.Hidden;
            torse.Visibility = Visibility.Hidden;
            brasgauche.Visibility = Visibility.Hidden;
            brasdroit.Visibility = Visibility.Hidden;
            jambegauche.Visibility = Visibility.Hidden;
            jambedroite.Visibility = Visibility.Hidden;
        }

        private void MettreAJourPendu()
        {
            switch (erreurs)
            {
                case 1:
                    tete.Visibility = Visibility.Visible;
                    break;
                case 2:
                    torse.Visibility = Visibility.Visible;
                    break;
                case 3:
                    brasgauche.Visibility = Visibility.Visible;
                    break;
                case 4:
                    brasdroit.Visibility = Visibility.Visible;
                    break;
                case 5:
                    jambegauche.Visibility = Visibility.Visible;
                    break;
                case 6:
                    jambedroite.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void BtnToggleMusique_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
            {
                mediaPlayer.Pause();
                BtnToggleMusique.Content = "▶️";
                isPlaying = false;
            }
            else
            {
                mediaPlayer.Play();
                BtnToggleMusique.Content = "⏸️";
                isPlaying = true;
            }
        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            mediaPlayer.Position = TimeSpan.Zero;
            mediaPlayer.Play();
        }
    }
}