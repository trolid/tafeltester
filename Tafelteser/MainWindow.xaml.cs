using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;

namespace RekenApp
{
    public partial class MainWindow : Window
    {
        private List<int> scores = new List<int>();
        private int currentLevel = 0;
        private int currentScore = 0;
        private int currentQuestion = 0;
        private Random random = new Random();
        private string[] levels = { "Niveau 1", "Niveau 2", "Niveau 3" };
        private char[] operators;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            string playerName = txtPlayerName.Text;
            currentLevel = cmbNiveau.SelectedIndex;

            if (currentLevel >= 0 && currentLevel < 3)
            {
                SetOperators(currentLevel);
                currentScore = 0;
                currentQuestion = 0;
                scores.Clear();

                for (int i = 0; i < 10; i++)
                {
                    string expression = GenerateExpression(currentLevel);
                    double correctAnswer = EvaluateExpression(expression);

                    // Controleer of het correcte antwoord een kommagetal is en sla de vraag over als dat het geval is
                    if (correctAnswer % 1 != 0)
                    {
                        i--;
                        continue;
                    }

                    // Toon de som aan de speler en vraag om een antwoord
                    int userAnswer = ShowQuestion(expression, correctAnswer);

                    // Controleer het antwoord en update de score
                    if (userAnswer == correctAnswer)
                    {
                        currentScore += 1;
                    }

                    scores.Add(userAnswer == correctAnswer ? 1 : 0);
                }

                // Toon de eindscore aan het einde van de sommen
                ShowEndScore(playerName, levels[currentLevel], currentScore, scores);
            }
            else
            {
                MessageBox.Show("Selecteer een geldig niveau.");
            }
        }

        private void SetOperators(int level)
        {
            switch (level)
            {
                case 0: // Niveau 1 (+ en -)
                    operators = new char[] { '+', '-' };
                    break;
                case 1: // Niveau 2 (+, -, *, /)
                    operators = new char[] { '+', '-', '*', '/' };
                    break;
                case 2: // Niveau 3 (+, -, *, /, (, ), en negatieve getallen)
                    operators = new char[] { '+', '-', '*', '/', '(', ')', '~' };
                    break;
            }
        }

        private string GenerateExpression(int level)
        {
            string expression = "";

            if (level == 2)
            {
                expression = GenerateComplexExpression();
            }
            else
            {
                expression = GenerateSimpleExpression(level == 0);
            }

            return expression;
        }

        private string GenerateSimpleExpression(bool allowNegative)
        {
            int num1 = random.Next(1, 11);
            int num2 = random.Next(1, allowNegative ? num1 : 11); // Zorg ervoor dat num2 kleiner is dan num1 in niveau 1
            return $"{num1} {GetOperatorString()} {num2}";
        }

        private string GenerateComplexExpression()
        {
            int num1 = random.Next(1, 11);
            int num2 = random.Next(1, 11);
            int num3 = random.Next(1, 11);
            char operator2 = GetOperatorString();
            return $"{num1} {operator2} ({num2} {GetOperatorString()} {num3})";
        }

        private double EvaluateExpression(string expression)
        {
            string sanitizedExpression = SanitizeExpression(expression);
            DataTable dt = new DataTable();
            try
            {
                var result = dt.Compute(sanitizedExpression, "");
                return double.Parse(result.ToString());
            }
            catch
            {
                return 0; // Als er een fout optreedt, geef 0 terug
            }
        }

        private string SanitizeExpression(string expression)
        {
            // Verwijder ongeldige tekens
            string sanitizedExpression = new string(expression.Where(c => Char.IsDigit(c) || operators.Contains(c)).ToArray());
            return sanitizedExpression;
        }

        private int ShowQuestion(string expression, double correctAnswer)
        {
            string question = expression;
            int userAnswer = -1;

            // Vraag de speler om een antwoord
            var answerString = Interaction.InputBox(question, "Rekenvraag", "");
            if (!int.TryParse(answerString, out userAnswer))
            {
                MessageBox.Show("Voer een geldig antwoord in.");
                return ShowQuestion(expression, correctAnswer);
            }

            // Toon of het antwoord goed of fout is
            string resultMessage = userAnswer == correctAnswer ? "Goed!" : "Fout!";
            MessageBox.Show($"Je antwoord is {resultMessage}", "Resultaat");

            return userAnswer;
        }

        private char GetOperatorString()
        {
            // Genereer een willekeurige operator als string
            char op = operators[random.Next(operators.Length)];
            if (op == '~')
            {
                return '-';
            }
            if (op == '(' || op == ')')
            {
                return '+';
            }
            return op;
        }

        private void ShowEndScore(string playerName, string level, int score, List<int> answers)
        {
            string message = $"Speler: {playerName}\nNiveau: {level}\nScore: {score}/10\n\n";
            for (int i = 0; i < answers.Count; i++)
            {
                message += $"Vraag {i + 1}: {answers[i]}\n";
            }

            MessageBox.Show(message, "Eindscore");
        }
    }
}
