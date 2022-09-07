using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ManagedReference
{
    public class ManagedReferenceGroupAttribute : System.Attribute
    {
        public string name;

        public ManagedReferenceGroupAttribute(string name): base()
        {
            this.name = name;
        }
    }
}
