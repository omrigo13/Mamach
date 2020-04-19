using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleCompiler
{
    public class Identifier : Token
    {
        public string Name { get; set; }
        public Identifier(string name, int line, int position)
        {
            Line = line;
            Position = position;
            Name = name;

            // if(char.IsDigit(name[0]) || name[0] == '_')
            //     throw new SyntaxErrorException("illigal identifier",this);
            // if(!name.All(char.IsLetterOrDigit))
            //     throw new SyntaxErrorException("illigal identifier",this);
            //you can add code here to identify invalid identifiers and throw an exception
        }
        public override bool Equals(object obj)
        {
            if (obj is Identifier)
            {
                Identifier t = (Identifier)obj;
                if (Name == t.Name)
                    return base.Equals(t);
            }
            return false;
        }
        public override string ToString()
        {
            return Name;
        }

    }
}
