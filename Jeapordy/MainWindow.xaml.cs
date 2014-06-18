using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;

namespace Jeopordy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void VoidDelegate();
        List<Category> Categories;
        Dictionary<string, QuestionAndAnswer> questionMapping;
        string _finalQuestion = "";
        string _finalAnswer = "";
        string _finalTopic = "";
        System.Timers.Timer timesUp;
        string ApplicationDir;

        public MainWindow()
        {
            InitializeComponent();
            Categories = new List<Category>();
            questionMapping = new Dictionary<string, QuestionAndAnswer>();
            ReadGameSheet();
            SetGameBoard();
            ApplicationDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            KeyDown += MainWindow_KeyDown;
            AudioPlayer.Source = new Uri(ApplicationDir + @"\Music\Jeopardy Board SMS.mp3");
            AudioPlayer.Play();
        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Environment.Exit(0);
            }
            if (e.Key == Key.F)
            {
                SetUpFinalJeopardy();
                
            }
        }

        public void ReadGameSheet()
        {
            
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".xml";
            if(ofd.ShowDialog() == true)
            {
                XDocument doc = XDocument.Load(ofd.FileName);
                foreach (var element in doc.Root.DescendantNodes().OfType<XElement>())
                {
                    if (element.Name == "category")
                    {
                        Category cat = new Category();
                        cat.CategoryName = (string)element.Attribute("desc");
                        foreach (var child in element.DescendantNodes().OfType<XElement>())
                        {
                            if (child.Name == "question")
                            {
                                QuestionAndAnswer qa = new QuestionAndAnswer();
                                qa.Answer = (string)child.Attribute("answer");
                                qa.Question = (string)child.Attribute("text");
                                qa.Value = int.Parse((string)child.Attribute("value"));
                                qa.DailyDouble = (string)child.Attribute("daily_double");
                                cat.AddQuestion(qa);
                            }
                        }
                        Categories.Add(cat);
                    }
                    if (element.Name == "final")
                    {
                        _finalQuestion = (string)element.Attribute("text");
                        _finalAnswer = (string)element.Attribute("answer");
                        _finalTopic = (string)element.Attribute("topic");
                    }
                }
            }
        }

        public void SetGameBoard()
        {
            for (int i = 0; i < 5; i++)
            {
                if(i == 0)
                    CategoryText1.Text = Categories[i].CategoryName;
                else if (i == 1)
                    CategoryText2.Text = Categories[i].CategoryName;
                else if (i == 2)
                    CategoryText3.Text = Categories[i].CategoryName;
                else if (i == 3)
                    CategoryText4.Text = Categories[i].CategoryName;
                else if (i == 4)
                    CategoryText5.Text = Categories[i].CategoryName;
                
                for (int j = 0; j < 5; j++)
                {
                    QuestionAndAnswer qa = Categories[i].questions[j];
                    string map = "QuestionC" + (i+1) + "R" + (j+1);
                    questionMapping[map] = qa;
                }
            }
        }

        private void Question_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock textblock = (TextBlock)sender;
            string question = questionMapping[textblock.Name].Question;
            string answer = questionMapping[textblock.Name].Answer;
            string dailydouble = questionMapping[textblock.Name].DailyDouble;

            if (textblock.Text == question)
            {
                textblock.Text = questionMapping[textblock.Name].Answer;
                timesUp.Stop();
            }
            else if (textblock.Text == answer)
            {
                textblock.Text = "";
            }
            else
            {
                if (dailydouble == "true")
                {
                    textblock.Text = "DAILY DOUBLE";
                    questionMapping[textblock.Name].DailyDouble = "false";
                    AudioPlayer.Source = new Uri(ApplicationDir + @"\Music\Daily double.mp3");
                    AudioPlayer.Play();
                }
                else
                {
                    textblock.Text = question;
                    timesUp = new System.Timers.Timer();
                    timesUp.AutoReset = false;
                    timesUp.Interval = 10000;
                    timesUp.Elapsed += timesUp_Elapsed;
                    timesUp.Enabled = true;
                }   
            }
        }

        public void SetUpFinalJeopardy()
        {
            Grid.SetZIndex(BlockQuestions, 99);
            Grid.SetZIndex(FinalJeopardyQuestion, 100);
            FinalJeopardyQuestion.Visibility = System.Windows.Visibility.Visible;
            FinalJeopardyQuestion.Text = _finalTopic;

           
        }

        void timesUp_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            VoidDelegate myDelegate = new VoidDelegate(delegate
            {
                AudioPlayer.Source = new Uri(ApplicationDir + @"\Music\Times up.mp3");
                AudioPlayer.Play();
            });
            this.Dispatcher.Invoke(DispatcherPriority.Normal, myDelegate);
        }

        private void FinalJeopardyQuestion_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (FinalJeopardyQuestion.Text == _finalTopic)
            {
                FinalJeopardyQuestion.Text = _finalQuestion;
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
                Thread.Sleep(3000);
                AudioPlayer.Source = new Uri(ApplicationDir + @"\Music\dumdidum.mp3");
                AudioPlayer.Play();
            }else  if (FinalJeopardyQuestion.Text == _finalQuestion)
            {
                FinalJeopardyQuestion.Text = _finalAnswer;
            }
        }
    }
}
