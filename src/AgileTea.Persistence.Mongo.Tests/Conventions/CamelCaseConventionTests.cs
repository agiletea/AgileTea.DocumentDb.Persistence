using System;
using AgileTea.Persistence.Common.Entities;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using Xunit;

namespace AgileTea.Persistence.Mongo.Tests.Conventions
{
    public class CamelCaseConventionTests
    {
        [Fact]
        public void GivenACamelCaseConventionPack_WhenAppliedToAClassInheritingFromIndexIdentityBase_ThenPropertyNamesComeOutAsCamelCase()
        {
            var convention = new CamelCaseElementNameConvention();

            var classMap = new BsonClassMap<TestClass>();
            convention.Apply(classMap.MapMember(x => x.FirstNames));
            convention.Apply(classMap.MapMember(x => x.LastName));
            convention.Apply(classMap.MapMember(x => x.Title));
            convention.Apply(classMap.MapMember(x => x.Salutation));

            Assert.Equal("firstNames", classMap.GetMemberMap(x => x.FirstNames).ElementName);
            Assert.Equal("lastName", classMap.GetMemberMap(x => x.LastName).ElementName);
            Assert.Equal("title", classMap.GetMemberMap(x => x.Title).ElementName);
            Assert.Equal("salutation", classMap.GetMemberMap(x => x.Salutation).ElementName);
        }

        public class TestClass : IndexedEntityBase
        {
            public string LastName { get; set; } = default!;
            public string FirstNames { get; set; } = default!;
            public string Title { get; set; }
            public string Salutation { get; set; } = default!;
            public DateTime? DateOfBirth { get; set; }
        }
    }
}
