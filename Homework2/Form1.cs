using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace Homework2
{
    // Main form class
    public partial class Form1 : Form
    {
        private string dataFilePath; // File path for the data file
        private int mapDimensions; // Dimensions of the SOM map
        private Map somMap; // Self-Organizing Map object
        private InstancesForm instancesForm; // Form to display instances

        // Constructor
        public Form1()
        {
            InitializeComponent();
        }

        // Method to handle Browse button click event
        private void BrowseButton_Click(object sender, EventArgs e)
        {
            // Open file dialog to select data file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                dataFilePath = openFileDialog.FileName; // Set the data file path
                FilePathTextBox.Text = dataFilePath; // Display file path in TextBox
            }
        }

        // Method to handle SOM button click event
        private void SOMButton_Click(object sender, EventArgs e)
        {
            // Check if file path and dimension value are provided
            if (string.IsNullOrEmpty(dataFilePath) || string.IsNullOrEmpty(DimensionTextBox.Text))
            {
                MessageBox.Show("Please select a file and enter a dimension value first."); // Show message box
                return;
            }

            // Parse dimension value
            if (!int.TryParse(DimensionTextBox.Text, out mapDimensions))
            {
                MessageBox.Show("Invalid dimension value. Please enter a valid integer."); // Show message box
                return;
            }

            // Calculate the number of clusters (dimension * dimension)
            int clusters = mapDimensions;

            // Initialize and run the SOM algorithm
            somMap = new Map(mapDimensions, clusters, dataFilePath);
            DisplayClusters(); // Display the clusters
        }

        // Method to display clusters on the panel
        private void DisplayClusters()
        {
            // Clear previous cluster buttons
            clusterPanel.Controls.Clear();

            // Set button size and spacing
            int buttonSize = 50;
            int spacing = 10;

            // Calculate panel width and height
            int panelWidth = (buttonSize + spacing) * somMap.GetLength() + spacing;
            int panelHeight = (buttonSize + spacing) * somMap.GetLength() + spacing;

            // Set panel size
            clusterPanel.Size = new Size(panelWidth, panelHeight);

            // Loop through each neuron in the map
            for (int i = 0; i < somMap.GetLength(); i++)
            {
                for (int j = 0; j < somMap.GetLength(); j++)
                {
                    // Create and configure cluster button
                    Button clusterButton = new Button();
                    clusterButton.Text = $"{i},{j}";
                    clusterButton.Tag = (i, j);
                    clusterButton.Size = new Size(buttonSize, buttonSize);
                    clusterButton.Location = new Point(i * (buttonSize + spacing), j * (buttonSize + spacing));
                    clusterButton.Click += ClusterButton_Click;

                    // Get instances in the cluster
                    List<string> instances = somMap.GetCluster(i, j);
                    if (instances != null && instances.Any())
                    {
                        // Display instances in tooltip
                        ToolTip toolTip = new ToolTip();
                        toolTip.SetToolTip(clusterButton, string.Join("\n", instances));
                    }

                    // Format button text
                    clusterButton.Text = $"{i}{j}";

                    // Add button to cluster panel
                    clusterPanel.Controls.Add(clusterButton);
                }
            }
        }

        // Method to handle cluster button click event
        private void ClusterButton_Click(object sender, EventArgs e)
        {
            // Get clicked cluster coordinates
            Button clusterButton = sender as Button;
            var (x, y) = ((int, int))clusterButton.Tag;

            // Show instances in the selected cluster
            ShowInstancesInCluster(x, y);
        }

        // Method to show instances in a cluster
        private void ShowInstancesInCluster(int x, int y)
        {
            // Get instances in the cluster
            List<string> instances = somMap.GetCluster(x, y);

            // Check if the cluster has instances
            if (instances == null || !instances.Any())
            {
                MessageBox.Show("The selected cluster does not contain any instances."); // Show message box
                return;
            }

            // Close previous instances form if exists
            if (instancesForm != null && !instancesForm.IsDisposed)
            {
                instancesForm.Close();
            }

            // Get field names from data file
            List<string> fieldNames = new List<string>();
            using (StreamReader reader = new StreamReader(dataFilePath))
            {
                string[] headers = reader.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    fieldNames.Add(header);
                }
            }

            // Create instances form and display instances
            instancesForm = new InstancesForm(instances, fieldNames);
            instancesForm.ShowDialog();
        }
    }

    // Class representing the Self-Organizing Map
    internal class Map
    {
        private Neuron[,] neurons; // Neurons grid
        private int iteration; // Current iteration
        private int length; // Length of the grid
        private int dimensions; // Dimensions of input data
        private Random random = new Random(); // Random object for weight initialization

        // Lists to store input data and patterns
        private List<string> labels = new List<string>();
        private List<double[]> patterns = new List<double[]>();
        private List<string> patternsWithTargetAttr = new List<string>();
        private List<double[]> originalPatterns = new List<double[]>();

        // Constructor
        public Map(int dimensions, int length, string file)
        {
            this.dimensions = dimensions;
            Initialise(length); // Initialize with the calculated length
            LoadData(file); // Load input data
            NormalisePatterns(); // Normalize input patterns
            Train(0.0000001); // Train the map
        }

        // Method to initialize the map
        private void Initialise(int length)
        {
            this.length = length;
            neurons = new Neuron[length, length];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    neurons[i, j] = new Neuron(i, j, length);
                    neurons[i, j].Weights = new double[dimensions];
                    neurons[i, j].TotalAttributeValues = new double[dimensions];
                    for (int k = 0; k < dimensions; k++)
                    {
                        neurons[i, j].Weights[k] = random.NextDouble();
                    }
                }
            }
        }

        // Method to load input data from file
        private void LoadData(string file)
        {
            StreamReader reader = File.OpenText(file);
            reader.ReadLine(); // Skip the first line
            while (!reader.EndOfStream)
            {
                string[] line = reader.ReadLine().Split(',');
                labels.Add(line[0]);
                double[] inputs = new double[dimensions];
                double[] originalInputs = new double[dimensions];
                for (int i = 0; i < dimensions; i++)
                {
                    inputs[i] = double.Parse(line[i], CultureInfo.InvariantCulture);

                    // Normalize inputs if necessary
                    if (line[i][0] == '.' || line[i][0] == ',')
                    {
                        line[i] = "0" + line[i];
                    }

                    originalInputs[i] = double.Parse(line[i], CultureInfo.InvariantCulture);
                }
                patterns.Add(inputs);
                originalPatterns.Add(originalInputs);
                patternsWithTargetAttr.Add(line[dimensions]);
            }
            reader.Close();
        }

        // Method to normalize input patterns
        private void NormalisePatterns()
        {
            for (int j = 0; j < dimensions; j++)
            {
                double max = 0;
                for (int i = 0; i < patterns.Count; i++)
                {
                    if (patterns[i][j] > max) max = patterns[i][j];
                }
                for (int i = 0; i < patterns.Count; i++)
                {
                    patterns[i][j] = patterns[i][j] / max;
                }
            }
        }

        // Method to train the map
        private void Train(double maxError)
        {
            double currentError = double.MaxValue;
            while (currentError > maxError)
            {
                currentError = 0;
                List<double[]> trainingSet = new List<double[]>();
                foreach (double[] pattern in patterns)
                {
                    trainingSet.Add(pattern);
                }
                for (int i = 0; i < patterns.Count; i++)
                {
                    double[] pattern = trainingSet[random.Next(patterns.Count - i)];
                    currentError += TrainPattern(pattern);
                    trainingSet.Remove(pattern);
                }
            }

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    neurons[i, j].Data = new List<double[]>();
                    neurons[i, j].Tr = new List<string>();
                }
            }
            for (int i = 0; i < patterns.Count; i++)
            {
                Neuron winner = Winner(patterns[i]);
                winner.AddInstance(patterns[i], patternsWithTargetAttr[i]);
            }
        }

        // Method to train a single pattern
        private double TrainPattern(double[] pattern)
        {
            double error = 0;
            Neuron winner = Winner(pattern);
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    error += neurons[i, j].UpdateWeights(pattern, winner, iteration);
                }
            }
            iteration++;
            return Math.Abs(error / (length * length));
        }

        // Method to find the winning neuron for a pattern
        private Neuron Winner(double[] pattern)
        {
            Neuron winner = null;
            double min = double.MaxValue;
            for (int i = 0; i < length; i++)
                for (int j = 0; j < length; j++)
                {
                    double d = Distance(pattern, neurons[i, j].Weights);
                    if (d < min)
                    {
                        min = d;
                        winner = neurons[i, j];
                    }
                }
            return winner;
        }

        // Method to calculate distance between two vectors
        private double Distance(double[] vector1, double[] vector2)
        {
            double value = 0;
            for (int i = 0; i < vector1.Length; i++)
            {
                value += Math.Pow((vector1[i] - vector2[i]), 2);
            }
            return Math.Sqrt(value);
        }

        // Method to get the length of the map
        public int GetLength()
        {
            return length;
        }

        // Method to get the instances in a cluster
        public List<string> GetCluster(int x, int y)
        {
            Neuron n = neurons[x, y];
            List<string> cluster = new List<string>();

            for (int i = 0; i < patterns.Count; i++)
            {
                Neuron winnerNeuron = Winner(patterns[i]);
                if (winnerNeuron.X == x && winnerNeuron.Y == y)
                {
                    StringBuilder instanceBuilder = new StringBuilder();
                    for (int j = 0; j < originalPatterns[i].Length; j++)
                    {
                        instanceBuilder.Append(originalPatterns[i][j]);
                        if (j < originalPatterns[i].Length - 1)
                            instanceBuilder.Append(", ");
                    }
                    cluster.Add(instanceBuilder.ToString() + ", " + patternsWithTargetAttr[i]);
                }
            }

            return cluster;
        }
    }

    // Class representing a single neuron in the map
    public class Neuron
    {
        public double[] Weights; // Weight vector
        public int X; // X coordinate of the neuron
        public int Y; // Y coordinate of the neuron
        private int length; // Length of the map
        private double nf; // Neighborhood factor
        public List<double[]> Data; // List of instances in the neuron
        public List<string> Tr; // List of target attributes

        public double[] TotalAttributeValues; // Total attribute values of instances in the neuron

        // Constructor
        public Neuron(int x, int y, int length)
        {
            X = x;
            Y = y;
            this.length = length;
            nf = 1000 / Math.Log(length);
            Data = new List<double[]>();
            Tr = new List<string>();
        }

        // Method to calculate Gaussian function
        private double Gauss(Neuron win, int it)
        {
            double distance = Math.Sqrt(Math.Pow(win.X - X, 2) + Math.Pow(win.Y - Y, 2));
            return Math.Exp(-Math.Pow(distance, 2) / (Math.Pow(Strength(it), 2)));
        }

        // Method to calculate learning rate
        private double LearningRate(int it)
        {
            return Math.Exp(-it / 1000) * 0.1;
        }

        // Method to calculate neighborhood strength
        private double Strength(int it)
        {
            return Math.Exp(-it / nf) * length;
        }

        // Method to update weights of the neuron
        public double UpdateWeights(double[] pattern, Neuron winner, int it)
        {
            double sum = 0;
            for (int i = 0; i < Weights.Length; i++)
            {
                double delta = LearningRate(it) * Gauss(winner, it) * (pattern[i] - Weights[i]);
                Weights[i] += delta;
                sum += delta;
            }
            return sum / Weights.Length;
        }

        // Method to add an instance to the neuron
        public void AddInstance(double[] instance, string label)
        {
            Data.Add(instance);
            Tr.Add(label);
        }
    }

    // Form to display instances in a cluster
    public partial class InstancesForm : Form
    {
        private System.Windows.Forms.Panel instancesPanel; // Panel to contain instances
        private System.Windows.Forms.ListBox listBox; // ListBox to display instances
        private System.Windows.Forms.Label dimensionLabel; // Label to display dimension

        // Constructor
        public InstancesForm(List<string> instances, List<string> fieldNames)
        {
            InitializeComponent();

            // Add field names label
            listBox.Items.Add(string.Join(", ", fieldNames));

            // Add instances to the list box
            foreach (var instance in instances)
            {
                listBox.Items.Add(instance);
            }

            // Set dimension label
            dimensionLabel.Text = $"Dimension: {fieldNames.Count}";

            // Create panel and add the list box to it
            instancesPanel = new Panel();
            instancesPanel.AutoScroll = true;
            listBox.Dock = DockStyle.Fill; // Fill the panel with the list box
            instancesPanel.Controls.Add(listBox);
            instancesPanel.Dock = DockStyle.Fill; // Fill the form with the panel
            this.Controls.Add(instancesPanel);
        }

        // Method to initialize components
        private void InitializeComponent()
        {
            this.listBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBox
            // 
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(12, 12);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(260, 238);
            this.listBox.TabIndex = 0;
            // 
            // InstancesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "InstancesForm";
            this.Text = "InstancesForm";
            this.ResumeLayout(false);

            // Add dimension label
            this.dimensionLabel = new System.Windows.Forms.Label();
            this.dimensionLabel.AutoSize = true;
            this.dimensionLabel.Location = new System.Drawing.Point(12, 46);
            this.dimensionLabel.Name = "DimensionLabel";
            this.dimensionLabel.Size = new System.Drawing.Size(60, 13);
            this.dimensionLabel.TabIndex = 5;
            this.dimensionLabel.Text = "Dimension:";
        }
    }
}
 