using System;
using System.IO;
using System.Text;

namespace CSVFileDetailsExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            // Define the output CSV file name
            string outputCsv = "Output.csv";

            // Validate if folder path is provided
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide the folder path as an argument.");
                return;
            }

            string folderPath = args[0];

            // Check if the folder exists
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("The folder path provided does not exist.");
                return;
            }

            // Create a new CSV file to store the results
            using (StreamWriter writer = new StreamWriter(outputCsv))
            {
                // Write the header row for the output CSV
                writer.WriteLine("Header Row,First Line,Last Line,File Size(KB),Row Count,Full File Path");

                // Start the recursive file search
                ProcessDirectory(folderPath, writer);
            }
        }

        // Function to process directories and files
        static void ProcessDirectory(string targetDirectory, StreamWriter writer)
        {
            // Get all CSV files in the current directory
            string[] fileEntries = Directory.GetFiles(targetDirectory, "*.csv");
            foreach (string fileName in fileEntries)
                ProcessFile(fileName, writer);

            // Recurse into subdirectories
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory, writer);
        }

        // Function to process each CSV file
        static void ProcessFile(string path, StreamWriter writer)
        {
            // Initialize variables
            string headerRow = string.Empty;
            string firstRow = string.Empty;
            string lastRow = string.Empty;
            int rowCount = 0;
            double fileSizeKB = new FileInfo(path).Length / 1024.0;

            // Read the file
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    rowCount++;

                    // Capture the header row (first row)
                    if (rowCount == 1)
                    {
                        headerRow = Escape(line);
                        continue;
                    }

                    // Capture the first row after header
                    if (rowCount == 2)
                    {
                        firstRow = Escape(line);
                    }

                    // Update the last row value
                    lastRow = Escape(line);
                }
            }

            // Log details to console
            Console.WriteLine($"Processing {path} - File Size: {fileSizeKB}KB, Row Count: {rowCount}");

            // Write to output CSV
            writer.WriteLine($"{headerRow},{firstRow},{lastRow},{fileSizeKB},{rowCount - 1},{Escape(path)}");
        }

// Function to escape special characters in CSV
        static string Escape(string s)
        {
            if (s.Contains(",") || s.Contains("\""))
            {
                s = "\"" + s.Replace("\"", "\"\"") + "\"";
            }

            return s;
        }
    }
}