using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace CharacterBuilder.ChampionsOnline.Model
{
    public class Statistics
    {
        public List<Attribute> Characteristics { get; set; }
        public List<Attribute> InnateTalents { get; set; }
        public List<Attribute> Talents { get; set; }

        public List<Power> TravelPowers { get; set; }

        public List<Powerset> Powersets { get; set; }

        public List<Statistic> Stats { get; set; }

        public Statistics()
        {
            Characteristics = new List<Attribute>();
            InnateTalents = new List<Attribute>();
            Talents = new List<Attribute>();

            TravelPowers = new List<Power>();
            Powersets = new List<Powerset>();
        }
        public void Serialize(string filename)
        {

            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(this.GetType());

                xmlSerializer.Serialize(fs, this);
            }
        }

        public string RunDiagnostic()
        {
            StringBuilder outputString = new StringBuilder("");

            // TEST POWERSETS FOR CODE CHARACTER PROGRESSION
            outputString.AppendLine("STARTING:  TESTING FOR CODE CHARACTER PROGRESSION");

            char previousCharacter = (char)48;
            char thisChar;

            //powersets
            foreach (Powerset thisPowerset in Powersets)
            {
                if (String.IsNullOrEmpty(thisPowerset.Code))
                    thisChar = (char)48;
                else
                    thisChar = thisPowerset.Code[0];
                
                // check that this powerset's code properly comes after the previous one
                if (Statistics.GetNumberFromCharacter(previousCharacter) != Statistics.GetNumberFromCharacter(thisChar) -1)
                {     
                    outputString.AppendLine(String.Format("Error in Powerset {0}: Progression from {1} to {2}",thisPowerset.Name, previousCharacter.ToString(),thisChar.ToString()));
                }
                previousCharacter = thisChar;
            }

            //powers in each powerset
            foreach (Powerset thisPowerset in Powersets)
            {
                previousCharacter = (char)48;
                foreach (Power thisPower in thisPowerset.Powers)
                {
                    if (String.IsNullOrEmpty(thisPower.Code))
                        thisChar = (char)48;
                    else
                        thisChar = thisPower.Code[0];
                    // check that this power's code properly comes after the previous one
                    if (Statistics.GetNumberFromCharacter(previousCharacter) != Statistics.GetNumberFromCharacter(thisChar) - 1)
                    {
                        outputString.AppendLine(String.Format("Error in Powerset {0}, Power {1}: Progression from {2} to {3}", thisPowerset.Name, thisPower.Name, previousCharacter.ToString(), thisChar.ToString()));
                    }
                    previousCharacter = thisChar;

                }
            }

            //advantages in each power
            foreach (Powerset thisPowerset in Powersets)
            {
                foreach (Power thisPower in thisPowerset.Powers)
                {
                    previousCharacter = (char)48;
                    foreach (Advantage thisAdvantage in thisPower.Advantages)
                    {
                        if (String.IsNullOrEmpty(thisAdvantage.Code))
                            thisChar = (char)48;
                        else
                            thisChar = thisAdvantage.Code[0];
                        // check that this power's code properly comes after the previous one
                        if (Statistics.GetNumberFromCharacter(previousCharacter) != Statistics.GetNumberFromCharacter(thisChar) - 1)
                        {
                            outputString.AppendLine(String.Format("Error in Powerset {0}, Power {1}, Advantage {2}: Progression from {3} to {4}", thisPowerset.Name, thisPower.Name, thisAdvantage.Name, previousCharacter.ToString(), thisChar.ToString()));
                        }
                        previousCharacter = thisChar;
                    }
                }
            }
            outputString.AppendLine("FINISHED:  TESTING FOR CODE CHARACTER PROGRESSION");

            outputString.AppendLine("STARTING:  TESTING FOR HTML TAG ERRORS");

            //powers in each powerset
            foreach (Powerset thisPowerset in Powersets)
            {
                foreach (Power thisPower in thisPowerset.Powers)
                {
                    string returnString = AnalyzeHTML(thisPower.Name, thisPower.Description);
                    if (returnString != String.Empty)
                        outputString.AppendLine(returnString);
                }
            }

            foreach (Power thisPower in TravelPowers)
            {
                string returnString = AnalyzeHTML(thisPower.Name, thisPower.Description);
                if (returnString != String.Empty)
                    outputString.AppendLine(returnString);
            }


            outputString.AppendLine("FINISHED:  TESTING FOR HTML TAG ERRORS");

            return outputString.ToString();
        }

        public static string AnalyzeHTML(string powerName, string inputString)
        {
            // this method checks the input string for HTML tags and returns the string error if the tags dont close properly
            // the <br> tag is ignored

            if (String.IsNullOrEmpty(inputString))
                return String.Empty;

            StateHTML currentState = StateHTML.Neutral;
            List<string> tags = new List<string>();
            string thisTag = String.Empty;
            for (int i = 0; i < inputString.Length; i++)
            {
                switch (currentState)
                {
                    case StateHTML.Neutral:
                        if (inputString[i] == (char)60)   //    <
                            currentState = StateHTML.TagStarted;
                        break;
                    case StateHTML.TagStarted:
                        if (inputString[i] == (char)47)   //    /
                        {
                            currentState = StateHTML.CloseTag;
                            thisTag = String.Empty;
                        }
                        else
                        {
                            currentState = StateHTML.OpenTag;
                            thisTag = inputString[i].ToString();
                        }
                        break;
                    case StateHTML.OpenTag:
                        if (inputString[i] != (char)62)
                            thisTag = String.Format("{0}{1}", thisTag, inputString[i]);
                        else  //    >
                        {
                            if (thisTag != "br")
                                tags.Add(thisTag);
                            currentState = StateHTML.Neutral;
                            thisTag = String.Empty;
                        }
                        break;
                    case StateHTML.CloseTag:
                        if (inputString[i] != (char)62)   
                            thisTag = String.Format("{0}{1}", thisTag, inputString[i]);
                        else  //    >
                        {
                            if (thisTag == tags[tags.Count - 1])
                            {
                                tags.RemoveAt(tags.Count - 1);
                                currentState = StateHTML.Neutral;
                            }
                            else
                                return String.Format("Error in power {0}: Tag <{1}> cannot be closed at character {2}", powerName, thisTag, i.ToString());
                        }

                        break;
                    default:
                        return String.Format("Problem with StateHTML in Power {0}, character {1}",powerName, i.ToString());
                }
            }

            // lastly, make sure there are no tags left
            if (tags.Count > 0)
                return String.Format("Error in power {0}: Tag {1} could not be closed", powerName, tags[tags.Count - 1]);
            if (currentState != StateHTML.Neutral)
                return String.Format("Error in power {0}: Tag not closed by end of string!", powerName);


            return String.Empty; ;
        }

        enum StateHTML : int
        {
            Neutral = 0,
            TagStarted = 1,
            OpenTag = 2,
            CloseTag = 3
        }

        public static int GetNumberFromCharacter(char inputCharacter)
        {
            // we use a single character to represent a powerset, power, advantage, etc
            // this character is used in the URL for later recall of the choices you made
            // 1-26 = a-z
            // 27-52 = A-Z
            // we never go over 52
            // 
            // This method receives the character, and returns the numeric value.
            // Zero is returned if there's a problem

            int asciiCode = (int)inputCharacter;

            if (asciiCode >= 97 && asciiCode <= 122)  // lowercase alphabet
                return asciiCode - 96;
            else if (asciiCode >= 65 && asciiCode <= 90)  // uppercase alphabet
                return asciiCode - 38;            
            else
                return 0;
        }

        public static Statistics Deserialize(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                XmlTextReader xmlTextReader = new XmlTextReader(fs);
                
                XmlSerializer xmlSerializer = new XmlSerializer( typeof(Statistics));

                return (Statistics)xmlSerializer.Deserialize(xmlTextReader);
            }
        }
    }
}
