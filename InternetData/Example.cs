using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


/*
  JSON (Java Script Object Notation)
  Concise, short-hand notation for transmitting the "state" of an object.
  That is, values for all of the Properties of a given instance of an object.
  This representation tells us only the structure of the Properties of the class,
  and nothing about the Behaviors that the class might have.

  This notation also need not be a /complete/ inventory of all of the possible
  properties (as we will see with the results of API calls)

  */

namespace InternetData
{
    /// <summary>
    /// A DataContract is the framework used to translate the "serialized" textual representation of an object
    /// Into the data structure stored in the computer's memory described by a class.
    ///
    /// Each property which is decorated with the DataMember flag becomes something that can be serialized
    /// and then loaded from text or converted to text.
    ///
    /// Standard built in Data Types, like int or string or even List are serializable, However, if we create new
    /// data structures, we have to tell the computer how to translate the new parts.
    ///
    /// This Example includes a nested set of classes.  Contact is a second class that we will have to define the
    /// DataContract for, as well as the DataContract representing my office (which I named Room)
    /// </summary>
    [DataContract]
    public class Example
    {
        #region Properties

        [DataMember]
        public string name;

        [DataMember]
        public Contact contact;


        #endregion // Properties

        #region Methods

        /// <summary>
        /// This override of the built in ToString method lets me program what happens when you include an instance
        /// of this class in a Console.WriteLine(example) statement.  Console.Write method's automatically call this
        /// ToString() method to decide what to display.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Name: {0}\nContact:\n\t{1}\nClasses:\n\t{2}",
                name,
                contact.ToString().Replace("\n", "\n\t"),
                this.ListClasses().Replace("\n", "\n\t"));
        }

        /// <summary>
        /// convert the list of classes into a printable string with each class printed on a separate line.
        /// </summary>
        /// <returns></returns>
        public string ListClasses()
        {
            string output = "";
            if (contact.classes != null)
            {
                foreach (string cl in contact.classes)
                {
                    output = string.Format("{0}\n{1}", output, cl);
                }
            }

            return output;
        }

        #endregion

        #region Static Methods

        public static Example LoadExample()
        {
            // Prepare to read the Example.json
            // FileStream is the object that we use to acomplish this task
            FileStream file = new FileStream("/Users/jcox/Desktop/Example.json", FileMode.Open);

            // In order to read the JSON we need to use a JSON Serializer object.
            // Generally, serializers are used to convert an object from computer
            // memory to a text representation suitable for transport, or vice-versa
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Example));

            // Now, we use the serializer object to Read the text file and convert it
            // into an Instance of the Example class.
            Example example = (Example)serializer.ReadObject(file);

            // Give that example back to the user who asked for it.
            return example;
        }

        #endregion // Methods
    }


    [DataContract]
    public class Contact
    {
        [DataMember]
        public string email;

        /// <summary>
        /// sometimes a data member may have a name that is not a valid
        /// variable name in C#.  This is how you handle those cases.
        /// Also, there's nothing stopping you from using this for
        /// other data members as well.
        /// </summary>
        [DataMember(Name = "office-phone")]
        public string officePhone;

        [DataMember]
        public Room office;

        [DataMember]
        public List<string> classes;
        /// <summary>
        /// Text output format.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Email: {0}\nOffice Phone: {1}\nOffice: {2}", email, officePhone, office);
        }
    }

    [DataContract]
    public class Room
    {
        [DataMember]
        public string building;

        [DataMember]
        public string room;

        /// <summary>
        /// get this as text that you can print.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} - {1}", building, room);
        }
    }
}
