using RSWareCommands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi;

namespace RSACommon.RecipeParser
{
    public class RecipProblemException : Exception
    {
        public RecipProblemException(string message) : base(message) 
        { 
        }

    }

    [Flags]
    public enum Job
    {
        Lav = 0b01,
        Au1 = 0b10,
        Au2 = 0b100,
    }

    public class Recipe
    {
        public string Model { get; set; } = string.Empty;
        public Job JobAvalaibility { get; set; } = Job.Lav;
        public int Index { get; set; } = 0;
        public string FullName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string LR { get; set; } = string.Empty;
        public static bool Comparer(Recipe first, Recipe second)
        {
            if(first == null || second == null) 
                return false;

            if(first.JobAvalaibility != second.JobAvalaibility)
                return false;

            if (first.Model != second.Model)
                return false;

            //if(first.LR != second.LR)
            //    return false;

            if (first.Index != second.Index)
                return false;

            return true;
        }

        public static string MakeRecipeName(Recipe toParse)
        {
            if (toParse is null)
                return string.Empty;

            string output = toParse.Name;

            switch(toParse.JobAvalaibility)
            {
                case Job.Lav:
                    output += "-001";
                    break;
                case Job.Au1:
                    output += "-010";
                    break;
                case Job.Au2:
                    output += "-100";
                    break;
                default:
                    throw new RecipProblemException($"tried to create {(int)toParse.JobAvalaibility} value for {toParse.FullName} recipe");
            }

            return output;
        }
    }

    public static class RecipeFinder
    {
        public static string[] ExtensionToSearch = new string[] { ".lav", ".au1", ".au2" };

        public static Recipe ExtractRecipeFromCommands(RecipeCommand command)
        {
            return ExtractRecipeFromCommands(command.CommandString);
        }

        public static Recipe ExtractRecipeFromCommands(string command)
        {
            if (command == null)
                return null;

            string[] splittedString = command.Split(new char[] { ',' });

            return ExtractRecipeFromMes(splittedString[1]);
        }

        public static Recipe ExtractRecipeFromFile(string recipeFilename)
        {
            Recipe r = new Recipe();
            FileInfo f = new FileInfo(recipeFilename);

            string nameWithoutExte = f.Name.Replace(f.Extension, "");
    
            string[] splittedString = nameWithoutExte.Split(new char[] { '-' });

            if (int.TryParse(splittedString[1], out int index))
            {
                r.Index = index;
                r.Model = splittedString[0].Substring(2);
                r.FileName = f.FullName;
                r.JobAvalaibility = FindType(f.Name);
                r.Name = nameWithoutExte;
                r.FullName = f.Name;
                r.LR = splittedString[2];
                return r;
            }

            return null;
        }

        public static Recipe ExtractRecipeFromMes(string recipeNameMes)
        {
            Recipe r = new Recipe();

            string[] splittedString = recipeNameMes.Split(new char[] { '-' });

            try
            {

                if (int.TryParse(splittedString[1], out int index)  && Job.TryParse(splittedString[3], out Job jobType))
                {
                    r.Index = index;
                    r.Model = splittedString[0].Substring(2);
                    r.JobAvalaibility = jobType;
                    r.LR = splittedString[2];

                    return r;
                }

            }
            catch
            {
                return null;
            }

            return null;
        }

        public static Job FindType(string filename)
        {
            FileInfo f = new FileInfo(filename);
            return FindType(f);
        }

        public static Job FindType(FileInfo f)
        {
            if (f.Extension.ToLower() == ExtensionToSearch[0])
                return Job.Lav;
            else if (f.Extension.ToLower() == ExtensionToSearch[1])
                return Job.Au1;
            else if (f.Extension.ToLower() == ExtensionToSearch[2])
                return Job.Au2;

            return Job.Lav;
        }


        public static List<Recipe> FindRecipeInFolder(string path)
        {
            return FindRecipeInFolder(path, ExtensionToSearch);
        }


        public static List<Recipe> FindRecipeInFolder(string path, string[] fileExtensions)
        {
            List<Recipe> ret = new List<Recipe>();

            if(!Directory.Exists(path))
                return ret;

            if (fileExtensions == null)
                return ret;


            string[] dizin = Directory.GetFiles(path, "*.*")
                .Where(f => fileExtensions.Contains(new FileInfo(f).Extension.ToLower())).ToArray();

            foreach(string filename in dizin)
            {
                if(File.Exists(filename))
                {
                    FileInfo f = new FileInfo(filename);

                    if (f.Length > 0)
                    {
                        Recipe r = ExtractRecipeFromFile(filename);
                        ret.Add(r);
                    }
                }
            }

            return ret;
        }
    }
}
