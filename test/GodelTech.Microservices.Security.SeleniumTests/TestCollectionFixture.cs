﻿using Xunit;

namespace GodelTech.Microservices.Security.SeleniumTests
{
    [CollectionDefinition("TestCollection")]
    public class TestCollectionFixture : ICollectionFixture<TestFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}