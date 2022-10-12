using NUnit.Framework;
using Queries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestQueries;

public class Tests
{
    Query query;

    public Tests()
    {
        this.query = new Query("postgres", "Python3.7", "yelpdb");
    }

    [SetUp]
    public void Setup()
    {
        query = new Query("postgres", "Python3.7", "yelpdb");
        query.Open();
    }

    [Test]
    public void TestStates()
    {
        var states = new HashSet<string>(query.GetAllStates());
        Console.WriteLine(string.Join(", ",states));
        Assert.AreNotEqual(0, states.Count);
    }
    [Test]
    public void TestBusinesses()
    {
        var businessData = query.GetBusinessesInZip(
            15203,
            new List<string>() {
                "Bars",
                "Mexican",
                "Tacos"
            },
            b =>
             (
                 (string) b["business_id"],
                 (string) b["business_name"],
                 (string) b["business_address"]
             )
        );

        var businesses = new List<(string,string,string)>(businessData);
        Assert.AreEqual(1, businesses.Count);
        Assert.AreEqual("The Urban Tap", businesses[0].Item2);
        Assert.AreEqual("1HfkEkNhI6XJXkgrCTBKpw", businesses[0].Item1);

    }
}
