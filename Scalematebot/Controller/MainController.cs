using Scalemate;
using Scalematebot.Model;
using Scalematebot.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scalematebot.Controller
{
    public class MainController
    {
        #region Properties
        public MainModel Model { get; private set; } = null;
        public MainView View { get; private set; } = null;
        public Tester Mate { get; private set; } = null;
        public string Question { get; private set; } = null;
        public Queue<string> Survey { get; private set; } = null;
        public List<string> SurveyAnswers { get; private set; } = null;
        public string[] Questions { get; private set; } = null;
        public string[] Options { get; private set; } = null;
        public List<string> Answers { get; private set; } = null;
        public int Index { get; private set; } = -1;
        public bool Running { get; private set; } = false;
        public Queue<string> Steps { get; private set; } = null;
        public string CurrentStep { get; private set; } = null;
        public bool FirstSurvey = true;
        public bool FirstQuestion = true;
        #endregion

        #region Constructor
        public MainController(MainModel model, MainView view)
        {
            // MVC setup
            Model = model;
            View = view;
            Mate = new Tester(Model, "procast-student-pt", View.Id);

            // Setting steps
            Steps = GetSteps();
            CurrentStep = NextStep();
        }
        #endregion

        #region General stuff
        public void Start()
        {
            Console.WriteLine("-- Starting new test!");
            
            // Preparing test
            Question = Mate.Question;
            Options = Mate.Options.Take(Mate.NoOptions).ToArray();
            Answers = new List<string>();
            FirstQuestion = true;

            // Preparing survey
            FirstSurvey = true;
            Survey = new Queue<string>(Mate.SurveyQuestions);
            SurveyAnswers = new List<string>();
        }
        

        public void Set(string step, string answer)
        {
            switch (step)
            {
                case "survey":
                    if (FirstSurvey)
                    {
                        FirstSurvey = false;
                    }
                    else
                    {
                        SurveyAnswers.Add(answer);
                        Console.WriteLine($"{step} {answer}");
                    }
                    break;
                case "test":
                    if (FirstQuestion)
                    {
                        FirstQuestion = false;
                    }
                    else
                    {
                        Mate.Listen(int.Parse(answer[0].ToString()) - 1);
                        Console.WriteLine($"{step} {answer}");
                    }
                    break;
            }
        }

        public void Save()
        {
            // Cleaning answers
            if (Mate.SurveyQuestions != null)
            {
                Mate.SurveyAnswers = SurveyAnswers.ToArray();
                Console.WriteLine(SurveyAnswers.Aggregate("Survey:", (box, it) => $"{box}\n{it}"));
            }
            Console.WriteLine(Mate.Answers.Aggregate("Answers:", (box, it) => $"{box}\n{it}"));

            // TODO Discover why it is not storing stuff correctly
            // Storing answers on memory
            var results = Mate.CalculateResults();
            Console.WriteLine(results);
        }
        #endregion

        #region Test methods
        public bool Ended()
        {
            return Mate.Ended;
        }

        public void NextQuestion()
        {
            Mate.Continue();
        }

        public string GetInstructions()
        {
            return Mate.BeginningInstructions.Aggregate("", (box, it) => $"{box} {it}");
        }
        #endregion

        #region Survey methods

        public string NextSurveyQuestion()
        {
            return (Survey.Count > 0) ? Survey.Dequeue() : null;
        }
        #endregion

        #region Steps methods
        public Queue<string> GetSteps()
        {
            Queue<string> steps = new Queue<string>();

            if (Mate.SurveyQuestions != null)
                steps.Enqueue("survey");
            if (Mate.BeginningInstructions != null)
                steps.Enqueue("instructions");
            steps.Enqueue("test");

            return steps;
        }

        public string NextStep()
        {
            return (Steps.Count > 0) ? Steps.Dequeue() : null;
        }

        public string GetThanks()
        {
            return (Mate.EndingInstructions != null) ? 
                Mate.EndingInstructions.Aggregate("", (box, it) => $"{box} {it}") : 
                "Obrigada por participar!";
        }
        #endregion
    }
}
