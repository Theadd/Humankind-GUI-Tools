using System;
using Modding.Humankind.DevTools;

namespace StyledGUI
{
    public class Database
    {
        public int Value;

        public Database(int value)
        {
            Value = value;
        }
    }

    public class DataContainerClass : IGrid<Database>
    {
        public Database Data { get; set; }

        public Database[] Databases;

        public DataContainerClass()
        {
            Databases = new Database[]
            {
                new Database(3), new Database(5), new Database(7)
            };
        }
    }

    public class ChainingMain
    {
        private DataContainerClass dataContainer = new DataContainerClass();

        public ChainingMain()
        {
            DerivedClass<Database> drawer = new DerivedClass<Database>()
            {
                DataContainer = dataContainer
            };

            if (drawer.C().D().C().A() is DerivedClass<Database> other)
            {
                other.E().A().B().E<Database>();
            }

            if ((drawer.A() as DerivedClass<Database>).C() == drawer)
            {
                Loggr.Log("EQUALS!!!", ConsoleColor.Red);
            }
            else
            {
                Loggr.Log("NOT EQUALS!!!", ConsoleColor.Red);
            }

        }
    }
}