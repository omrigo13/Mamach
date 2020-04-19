using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    public class Assembler
    {
        private const int WORD_SIZE = 16;

        private Dictionary<string, int[]> m_dControl, m_dJmp, m_dest; //these dictionaries map command mnemonics to machine code - they are initialized at the bottom of the class

        private Dictionary<string, int> m_symbolsTable, m_symbolsTable2;
        //more data structures here (symbol map, ...)

        public Assembler()
        {
            InitCommandDictionaries();
        }

        //this method is called from the outside to run the assembler translation
        public void TranslateAssemblyFile(string sInputAssemblyFile, string sOutputMachineCodeFile)
        {
            //read the raw input, including comments, errors, ...
            StreamReader sr = new StreamReader(sInputAssemblyFile);
            List<string> lLines = new List<string>();
            while (!sr.EndOfStream)
            {
                lLines.Add(sr.ReadLine());
            }
            sr.Close();
            //translate to machine code
            List<string> lTranslated = TranslateAssemblyFile(lLines);
            //write the output to the machine code file
            StreamWriter sw = new StreamWriter(sOutputMachineCodeFile);
            foreach (string sLine in lTranslated)
                sw.WriteLine(sLine);
            sw.Close();
        }

        //translate assembly into machine code
        private List<string> TranslateAssemblyFile(List<string> lLines)
        {
            //implementation order:
            //first, implement "TranslateAssemblyToMachineCode", and check if the examples "Add", "MaxL" are translated correctly.
            //next, implement "CreateSymbolTable", and modify the method "TranslateAssemblyToMachineCode" so it will support symbols (translating symbols to numbers). check this on the examples that don't contain macros
            //the last thing you need to do, is to implement "ExpendMacro", and test it on the example: "SquareMacro.asm".
            //init data structures here 

            //expand the macros
            List<string> lAfterMacroExpansion = ExpendMacros(lLines);

            //first pass - create symbol table and remove lable lines
            CreateSymbolTable(lAfterMacroExpansion);

            //second pass - replace symbols with numbers, and translate to machine code
            List<string> lAfterTranslation = TranslateAssemblyToMachineCode(lAfterMacroExpansion);
            return lAfterTranslation;
        }

        
        //first pass - replace all macros with real assembly
        private List<string> ExpendMacros(List<string> lLines)
        {
            //You do not need to change this function, you only need to implement the "ExapndMacro" method (that gets a single line == string)
            List<string> lAfterExpansion = new List<string>();
            for (int i = 0; i < lLines.Count; i++)
            {
                //remove all redudant characters
                string sLine = CleanWhiteSpacesAndComments(lLines[i]);
                if (sLine == "")
                    continue;
                //if the line contains a macro, expand it, otherwise the line remains the same
                List<string> lExpanded = ExapndMacro(sLine);
                //we may get multiple lines from a macro expansion
                foreach (string sExpanded in lExpanded)
                {
                    lAfterExpansion.Add(sExpanded);
                }
            }
            return lAfterExpansion;
        }

        //expand a single macro line
        private List<string> ExapndMacro(string sLine)
        {
            List<string> lExpanded = new List<string>();

            if (IsCCommand(sLine))
            {
                string sDest, sCompute, sJmp;
                GetCommandParts(sLine, out sDest, out sCompute, out sJmp);
                //your code here - check for indirect addessing and for jmp shortcuts
                //read the word file to see all the macros you need to support

                if (sCompute.Contains("A+A") || sCompute.Contains("D+D") || sCompute.Contains("M+A") || sCompute.Contains("M+M") || sCompute.Contains("A+M"))
                    throw new ArgumentException("invalid operation");
                
                if (sCompute.Contains("A-A") || sCompute.Contains("D-D") || sCompute.Contains("M-A") || sCompute.Contains("M-M") || sCompute.Contains("A-M"))
                    throw new ArgumentException("invalid operation");
                
                if(sDest.Contains("AA") || sDest.Contains("MM") || sDest.Contains("DD") || sDest.Contains("MAD") || sDest.Contains("MDA") || sDest.Contains("ADM")
                   || sDest.Contains("MA") || sDest.Contains("DM") || sDest.Contains("DA"))
                    throw new ArgumentException("invalid operation");
                
                if(!m_dest.ContainsKey(sDest) && !m_dJmp.ContainsKey(sJmp) && !m_dControl.ContainsKey(sCompute) && !m_symbolsTable.ContainsKey(sCompute))
                    throw new ArgumentException("invalid operation");

                if(sDest.Contains("+A") || sDest.Contains("+M") || sDest.Contains("+D") ||
                   sDest.Contains("A+") || sDest.Contains("M+") || sDest.Contains("D+") ||
                   sDest.Contains("-A") || sDest.Contains("-M") || sDest.Contains("-D") ||
                   sDest.Contains("A-") || sDest.Contains("M-") || sDest.Contains("D-"))
                    throw new ArgumentException("invalid operation");
                
                string dest;
                int immidiateInt;
                if(m_dControl.ContainsKey(sCompute) && m_dest.ContainsKey(sDest) && m_dJmp.ContainsKey(sJmp))
                    lExpanded.Add(sLine);
                else
                {
                    if (sCompute.Contains("++") && sDest == "" && !m_dControl.ContainsKey(sCompute)) //increment
                    {
                        dest = sCompute.Substring(0, sCompute.IndexOf('+'));
                        if (m_dest.ContainsKey(dest)) // dest is A/M/D and not a variable
                            lExpanded.Add(dest + "=" + dest + "+1");
                        else // dest is a variable
                        {
                            lExpanded.Add("@" + dest);
                            lExpanded.Add("M=M+1");
                        }
                    }

                    else if (sCompute.Contains("--") && sDest == "" && !m_dControl.ContainsKey(sCompute)) //decrement
                    {
                        dest = sCompute.Substring(0, sCompute.IndexOf('-'));
                        if (m_dest.ContainsKey(dest)) // dest is A/M/D and not a variable
                            lExpanded.Add(dest + "=" + dest + "-1");
                        else // dest is a variable
                        {
                            lExpanded.Add("@" + dest);
                            lExpanded.Add("M=M-1");
                        }
                    }

                    /*else if (sLine.Contains('=') && !m_dControl.ContainsKey(sCompute) &&
                             Int32.TryParse(sCompute, out immidiateInt)) // direct+immediate addressing
                    {
                        if (m_dest.ContainsKey(sDest)) // dest=AMD
                        {
                            lExpanded.Add("@" + sCompute);
                            lExpanded.Add(sDest + "=A");
                        }
                        else // dest=variable
                        {
                            lExpanded.Add("@" + sCompute);
                            lExpanded.Add("D=A");
                            lExpanded.Add("@" + sDest);
                            lExpanded.Add("M=D");
                        }
                    }*/
                
                /*else if (sLine.Contains(':') && !m_dJmp.ContainsKey(sJmp)) // Jump
                {
                    lExpanded.Add("@" + sJmp.Substring(sJmp.IndexOf(':') + 1)); //access the label place
                    lExpanded.Add(sCompute + ";" + sJmp.Substring(0, sJmp.IndexOf(':'))); // condition of jump
                }*/
                    else if(!m_dest.ContainsKey(sDest))
                    {
                        if (!m_dControl.ContainsKey(sCompute))
                        {
                            if (sLine.Contains('=') && Int32.TryParse(sCompute, out immidiateInt))
                            {
                                lExpanded.Add("@" + sCompute);
                                lExpanded.Add("D=A");
                                lExpanded.Add("@" + sDest);
                                lExpanded.Add("M=D");
                            }
                            else
                            {
                                lExpanded.Add("@" + sCompute);
                                lExpanded.Add("D=M");
                                lExpanded.Add("@" + sDest);
                                lExpanded.Add("M=D");
                            }
                        }
                        else
                        {
                            lExpanded.Add("@" + sDest);
                            lExpanded.Add("M=" + sCompute);
                        }
                    }
                    else
                    {
                        
                        if (!m_dControl.ContainsKey(sCompute))
                        {
                            if (sLine.Contains('=') && Int32.TryParse(sCompute, out immidiateInt))
                            {
                                lExpanded.Add("@" + sCompute);
                                lExpanded.Add(sDest + "=A");
                            }
                            else
                            {
                                lExpanded.Add("@" + sCompute);
                                lExpanded.Add(sDest + "=M");    
                            }
                        }

                        if (sLine.Contains(':') && !m_dJmp.ContainsKey(sJmp)) // Jump
                        {
                            lExpanded.Add("@" + sJmp.Substring(sJmp.IndexOf(':') + 1)); //access the label place
                            lExpanded.Add(sCompute + ";" + sJmp.Substring(0, sJmp.IndexOf(':'))); // condition of jump
                        }
                    }
                }
        
                //direct addressing
                /*else if (!m_dControl.ContainsKey(sCompute) && !m_dest.ContainsKey(sDest)) // compute and dest are variables
                {
                    lExpanded.Add("@" + sCompute);
                    lExpanded.Add("D=M");
                    lExpanded.Add("@" + sDest);
                    lExpanded.Add("M=D");
                }
                
                //direct addressing
                else if (!m_dControl.ContainsKey(sCompute) && m_dest.ContainsKey(sDest)) // compute=variable and dest=AMD
                {
                    lExpanded.Add("@" + sCompute);
                    lExpanded.Add(sDest + "=M");
                }
                
                //direct addressing
                else if (m_dControl.ContainsKey(sCompute) && !m_dest.ContainsKey(sDest)) // compute=AMD and dest=variable
                {
                    lExpanded.Add("@" + sDest);
                    lExpanded.Add("M=" + sCompute);
                }*/
            }
            if (lExpanded.Count == 0)
                lExpanded.Add(sLine);
            return lExpanded;
        }

        //second pass - record all symbols - labels and variables
        private void CreateSymbolTable(List<string> lLines)
        {
            string sLine = "";
            
            int variable_place = 0;
            int label_place = 16;
            m_symbolsTable = new Dictionary<string, int>();
            m_symbolsTable2 = new Dictionary<string, int>();
            
            //init symbols table of R0-R15
            m_symbolsTable["R0"] = 0;
            m_symbolsTable["R1"] = 1;
            m_symbolsTable["R2"] = 2;
            m_symbolsTable["R3"] = 3;
            m_symbolsTable["R4"] = 4;
            m_symbolsTable["R5"] = 5;
            m_symbolsTable["R6"] = 6;
            m_symbolsTable["R7"] = 7;
            m_symbolsTable["R8"] = 8;
            m_symbolsTable["R9"] = 9;
            m_symbolsTable["R10"] = 10;
            m_symbolsTable["R11"] = 11;
            m_symbolsTable["R12"] = 12;
            m_symbolsTable["R13"] = 13;
            m_symbolsTable["R14"] = 14;
            m_symbolsTable["R15"] = 15;
            
            for (int i = 0; i < lLines.Count; i++)
            {
                sLine = lLines[i];
                
                if (sLine.Contains("A+A") || sLine.Contains("D+D") || sLine.Contains("M+A") || sLine.Contains("M+M") || sLine.Contains("A+M"))
                    throw new ArgumentException("invalid operation");
                
                if (sLine.Contains("A-A") || sLine.Contains("D-D") || sLine.Contains("M-A") || sLine.Contains("M-M") || sLine.Contains("A-M"))
                    throw new ArgumentException("invalid operation");
                
                if(sLine.Contains("AA") || sLine.Contains("MM") || sLine.Contains("DD") || sLine.Contains("MAD") || sLine.Contains("MDA") || sLine.Contains("ADM")
                   || sLine.Contains("MA") || sLine.Contains("DM") || sLine.Contains("DA"))
                    throw new ArgumentException("invalid operation");
                
                if (IsLabelLine(sLine))
                {
                    //record label in symbol table
                    //do not add the label line to the result
                    string label = sLine.Substring(1, sLine.Length - 2);
                    if(label[0] >= '0' && label[0] <= '9')
                        throw new ArgumentException("label cannot start with a number");

                    if (m_symbolsTable.ContainsKey(label))
                    {
                        if (!m_symbolsTable2.ContainsKey(label))
                        {
                            m_symbolsTable[label] = i - variable_place;
                            m_symbolsTable2[label] = 1;
                            variable_place++;
                        }
                        else 
                            throw new ArgumentException("the label is already exist");
                    }
                    else
                    {
                        m_symbolsTable[label] = i - variable_place;
                        m_symbolsTable2[label] = 1;
                        variable_place++;
                    }
                }
                
                else if (IsACommand(sLine))
                {
                    //may contain a variable - if so, record it to the symbol table (if it doesn't exist there yet...)
                    String acmd = sLine.Substring(1);
                    int number;
                    if (!Int32.TryParse(acmd, out number))
                    {
                        if ((sLine[1] >= '0' && sLine[1] <= '9')) 
                            throw new ArgumentException("label cannot start with a number");
                        
                        if(!m_symbolsTable.ContainsKey(acmd))
                        {
                            m_symbolsTable[acmd] = label_place;
                            label_place++;
                        }
                    }
                }
                else if (IsCCommand(sLine))
                {
                    //do nothing here
                }
                else
                    throw new FormatException("Cannot parse line " + i + ": " + lLines[i]);
            }
        }
        
        //third pass - translate lines into machine code, replacing symbols with numbers
        private List<string> TranslateAssemblyToMachineCode(List<string> lLines)
        {
            string sLine = "";
            List<string> lAfterPass = new List<string>();
            for (int i = 0; i < lLines.Count; i++)
            {
                sLine = lLines[i];
                
                if (sLine.Contains("A+A") || sLine.Contains("D+D") || sLine.Contains("M+A") || sLine.Contains("M+M") || sLine.Contains("A+M"))
                    throw new ArgumentException("invalid operation");
                
                if (sLine.Contains("A-A") || sLine.Contains("D-D") || sLine.Contains("M-A") || sLine.Contains("M-M") || sLine.Contains("A-M"))
                    throw new ArgumentException("invalid operation");
                
                if(sLine.Contains("AA") || sLine.Contains("MM") || sLine.Contains("DD") || sLine.Contains("MAD") || sLine.Contains("MDA") || sLine.Contains("ADM")
                   || sLine.Contains("MA") || sLine.Contains("DM") || sLine.Contains("DA"))
                    throw new ArgumentException("invalid operation");
                
                if (IsACommand(sLine))
                {
                    //translate an A command into a sequence of bits
                    sLine = sLine.Substring(1);
                    int intsLine;
                    if(Int32.TryParse(sLine, out intsLine))
                        lAfterPass.Add(ToBinary(intsLine));
                    else if(m_symbolsTable.ContainsKey(sLine))
                        lAfterPass.Add(ToBinary(m_symbolsTable[sLine]));
                    else
                        throw new ArgumentException("the label isn't in the dictionary");    
                }
                else if (IsCCommand(sLine))
                {
                    string sDest, sControl, sJmp;
                    GetCommandParts(sLine, out sDest, out sControl, out sJmp);
                    //translate an C command into a sequence of bits
                    //take a look at the dictionaries m_dControl, m_dJmp, and where they are initialized (InitCommandDictionaries), to understand how to you them here
                    
                    if(!m_dControl.ContainsKey(sControl) || !m_dJmp.ContainsKey(sJmp) || !m_dest.ContainsKey(sDest))
                        throw new ArgumentException("invalid operation");
                    
                    int[] control = m_dControl[sControl];
                    int[] jmp = m_dJmp[sJmp];
                    int[] dest = m_dest[sDest];
                    lAfterPass.Add("111" + ToString(control) + ToString(dest) + ToString(jmp));
                }
                else if (IsLabelLine(sLine))
                {
                    String label = sLine.Substring(1, sLine.Length - 2);
                    if (!m_symbolsTable.ContainsKey(label))
                        throw new ArgumentException("the label isn't in the dictionary");
                }
                else
                    throw new FormatException("Cannot parse line " + i + ": " + lLines[i]);
            }
            
            return lAfterPass;
        }

        //helper functions for translating numbers or bits into strings
        private string ToString(int[] aBits)
        {
            string sBinary = "";
            for (int i = 0; i < aBits.Length; i++)
                sBinary += aBits[i];
            return sBinary;
        }

        private string ToBinary(int x)
        {
            string sBinary = "";
            for (int i = 0; i < WORD_SIZE; i++)
            {
                sBinary = (x % 2) + sBinary;
                x = x / 2;
            }
            return sBinary;
        }


        //helper function for splitting the various fields of a C command
        private void GetCommandParts(string sLine, out string sDest, out string sControl, out string sJmp)
        {
            if (sLine.Contains('='))
            {
                int idx = sLine.IndexOf('=');
                sDest = sLine.Substring(0, idx);
                sLine = sLine.Substring(idx + 1);
            }
            else
                sDest = "";
            if (sLine.Contains(';'))
            {
                int idx = sLine.IndexOf(';');
                sControl = sLine.Substring(0, idx);
                sJmp = sLine.Substring(idx + 1);

            }
            else
            {
                sControl = sLine;
                sJmp = "";
            }
        }

        private bool IsCCommand(string sLine)
        {
            return !IsLabelLine(sLine) && sLine[0] != '@';
        }

        private bool IsACommand(string sLine)
        {
            return sLine[0] == '@';
        }

        private bool IsLabelLine(string sLine)
        {
            if (sLine.StartsWith("(") && sLine.EndsWith(")"))
                return true;
            return false;
        }

        private string CleanWhiteSpacesAndComments(string sDirty)
        {
            string sClean = "";
            for (int i = 0 ; i < sDirty.Length ; i++)
            {
                char c = sDirty[i];
                if (c == '/' && i < sDirty.Length - 1 && sDirty[i + 1] == '/') // this is a comment
                    return sClean;
                if (c > ' ' && c <= '~')//ignore white spaces
                    sClean += c;
            }
            return sClean;
        }


        private void InitCommandDictionaries()
        {
            m_dControl = new Dictionary<string, int[]>();

            m_dControl["0"] = new int[] { 0, 1, 0, 1, 0, 1, 0 };
            m_dControl["1"] = new int[] { 0, 1, 1, 1, 1, 1, 1 };
            m_dControl["-1"] = new int[] { 0, 1, 1, 1, 0, 1, 0 };
            m_dControl["D"] = new int[] { 0, 0, 0, 1, 1, 0, 0 };
            m_dControl["A"] = new int[] { 0, 1, 1, 0, 0, 0, 0 };
            m_dControl["!D"] = new int[] { 0, 0, 0, 1, 1, 0, 1 };
            m_dControl["!A"] = new int[] { 0, 1, 1, 0, 0, 0, 1 };
            m_dControl["-D"] = new int[] { 0, 0, 0, 1, 1, 1, 1 };
            m_dControl["-A"] = new int[] { 0, 1, 1, 0, 0,1, 1 };
            m_dControl["D+1"] = new int[] { 0, 0, 1, 1, 1, 1, 1 };
            m_dControl["A+1"] = new int[] { 0, 1, 1, 0, 1, 1, 1 };
            m_dControl["D-1"] = new int[] { 0, 0, 0, 1, 1, 1, 0 };
            m_dControl["A-1"] = new int[] { 0, 1, 1, 0, 0, 1, 0 };
            m_dControl["D+A"] = new int[] { 0, 0, 0, 0, 0, 1, 0 };
            m_dControl["D-A"] = new int[] { 0, 0, 1, 0, 0, 1, 1 };
            m_dControl["A-D"] = new int[] { 0, 0, 0, 0, 1,1, 1 };
            m_dControl["D&A"] = new int[] { 0, 0, 0, 0, 0, 0, 0 };
            m_dControl["D|A"] = new int[] { 0, 0, 1, 0,1, 0, 1 };

            m_dControl["M"] = new int[] { 1, 1, 1, 0, 0, 0, 0 };
            m_dControl["!M"] = new int[] { 1, 1, 1, 0, 0, 0, 1 };
            m_dControl["-M"] = new int[] { 1, 1, 1, 0, 0, 1, 1 };
            m_dControl["M+1"] = new int[] { 1, 1, 1, 0, 1, 1, 1 };
            m_dControl["M-1"] = new int[] { 1, 1, 1, 0, 0, 1, 0 };
            m_dControl["D+M"] = new int[] { 1, 0, 0, 0, 0, 1, 0 };
            m_dControl["D-M"] = new int[] { 1, 0, 1, 0, 0, 1, 1 };
            m_dControl["M-D"] = new int[] { 1, 0, 0, 0, 1, 1, 1 };
            m_dControl["D&M"] = new int[] { 1, 0, 0, 0, 0, 0, 0 };
            m_dControl["D|M"] = new int[] { 1, 0, 1, 0, 1, 0, 1 };
            
            m_dControl["M+D"] = new int[] { 1, 0, 0, 0, 0, 1, 0 };
            m_dControl["A+D"] = new int[] { 0, 0, 0, 0, 0, 1, 0 };

            m_dJmp = new Dictionary<string, int[]>();

            m_dJmp[""] = new int[] { 0, 0, 0 };
            m_dJmp["JGT"] = new int[] { 0, 0, 1 };
            m_dJmp["JEQ"] = new int[] { 0, 1, 0 };
            m_dJmp["JGE"] = new int[] { 0, 1, 1 };
            m_dJmp["JLT"] = new int[] { 1, 0, 0 };
            m_dJmp["JNE"] = new int[] { 1, 0, 1 };
            m_dJmp["JLE"] = new int[] { 1, 1, 0 };
            m_dJmp["JMP"] = new int[] { 1, 1, 1 };
            
            m_dest = new Dictionary<string, int[]>();
            
            m_dest[""] = new int[] { 0, 0, 0 };
            m_dest["M"] = new int[] { 0, 0, 1 };
            m_dest["D"] = new int[] { 0, 1, 0 };
            m_dest["MD"] = new int[] { 0, 1, 1 };
            m_dest["A"] = new int[] { 1, 0, 0 };
            m_dest["AM"] = new int[] { 1, 0, 1 };
            m_dest["AD"] = new int[] { 1, 1, 0 };
            m_dest["AMD"] = new int[] { 1, 1, 1 };
        }
    }
}
