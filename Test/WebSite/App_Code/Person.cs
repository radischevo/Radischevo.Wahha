using System;
using System.Collections.Generic;

public class Person
{
    private int _id;
    private string _firstName;
    private string _lastName;
    private string _middleName;
    private string _address;

    public Person()
    {
        _firstName = String.Empty;
        _lastName = String.Empty;
        _middleName = String.Empty;
        _address = String.Empty;
    }

    public int ID
    {
        get
        {
            return _id;
        }
    }

    public string FirstName
    {
        get
        {
            return _firstName;
        }
        set
        {
            _firstName = value;
        }
    }

    public string LastName
    {
        get
        {
            return _lastName;
        }
        set
        {
            _lastName = value;
        }
    }

    public string MiddleName
    {
        get
        {
            return _middleName;
        }
        set
        {
            _middleName = value;
        }
    }

    public string Address
    {
        get
        {
            return _address;
        }
        set
        {
            _address = value;
        }
    }

    public static List<Person> GetTempBunch(int count)
    {
        List<Person> list = new List<Person>();

        for (int i = 0; i < count; ++i)
        {
            Person p = new Person();
            p._id = i + 1;
            p._firstName = String.Format("Mike-{0}", i);
            p._lastName = String.Format("Aurelli-{0}", i);
            p._middleName = "B.";
            p._address = String.Format("MF {0} in => {1} c", i * 10, i * 20);

            list.Add(p);
        }
        return list;
    }
}