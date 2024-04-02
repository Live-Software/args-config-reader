using ArgsConfigReader;

namespace ArgsConfigReaderTest;

public class Tests {

    private class TestConfig(string testStringProperty, int testIntProperty, bool testSoloBoolProperty, string testOverwriteProperty) {
        [ConfigProperty(shortName = "p", longName = "property", defaultValue = "default", description = "A property")]
        public string testStringProperty { get; } = testStringProperty;

        [ConfigProperty(shortName = "i", longName = "int", defaultValue = "12", description = "An int property")]
        public int testIntProperty { get; } = testIntProperty;

        [ConfigProperty(shortName = "b", longName = "bool", defaultValue = "false", solo = true)]
        public bool testSoloBoolProperty { get; } = testSoloBoolProperty;

        [ConfigProperty(shortName = "o", longName = "overwrite", defaultValue = "def", description = "An overwriteable property", overwriteFlag = "w", overwriteValue = "overwritten")]
        public string testOverwriteProperty { get; } = testOverwriteProperty;
    }

    [Test]
    public void readsStringShortProperty() {
        var res = ConfigReader.readConfigFromArgs<TestConfig>("-p", "propertyyy");
        Assert.That(res, Is.Not.Null);
        Assert.That(res!.testStringProperty, Is.EqualTo("propertyyy"));
    }

    [Test]
    public void readsStringLongProperty() {
        var res = ConfigReader.readConfigFromArgs<TestConfig>("--property", "propertyyy");
        Assert.That(res, Is.Not.Null);
        Assert.That(res!.testStringProperty, Is.EqualTo("propertyyy"));
    }

    [Test]
    public void usesDefaultStringValue() {
        var res = ConfigReader.readConfigFromArgs<TestConfig>();
        Assert.That(res, Is.Not.Null);
        Assert.That(res!.testStringProperty, Is.EqualTo("default"));
    }

    [Test]
    public void usesFirstOccurrence() {
        var res = ConfigReader.readConfigFromArgs<TestConfig>("-p", "first", "-p", "different");
        Assert.That(res, Is.Not.Null);
        Assert.That(res!.testStringProperty, Is.EqualTo("first"));
    }

    [Test]
    public void readsIntShortProperty() {
        var res = ConfigReader.readConfigFromArgs<TestConfig>("-i", "2");
        Assert.That(res, Is.Not.Null);
        Assert.That(res!.testIntProperty, Is.EqualTo(2));
    }

    [Test]
    public void readsIntLongProperty() {
        var res = ConfigReader.readConfigFromArgs<TestConfig>("--int", "2");
        Assert.That(res!.testIntProperty, Is.EqualTo(2));
    }

    [Test]
    public void usesDefaultIntValue() {
        var res = ConfigReader.readConfigFromArgs<TestConfig>();
        Assert.That(res, Is.Not.Null);
        Assert.That(res!.testIntProperty, Is.EqualTo(12));
    }

    [Test]
    public void readsMultipleValues() {
        var res = ConfigReader.readConfigFromArgs<TestConfig>("-p", "first", "-i", "2");
        Assert.That(res, Is.Not.Null);
        Assert.Multiple(() => {
            Assert.That(res!.testStringProperty, Is.EqualTo("first"));
            Assert.That(res.testIntProperty, Is.EqualTo(2));
        });
    }

    [Test]
    public void setSoloPropertyToTrue() {
        var res = ConfigReader.readConfigFromArgs<TestConfig>("-b");
        Assert.That(res, Is.Not.Null);
        Assert.Multiple(() => { Assert.That(res!.testSoloBoolProperty, Is.True); });
    }

    [Test]
    public void usesOverwrite() {
        var res = ConfigReader.readConfigFromArgs<TestConfig>("-o", "this", "-w");
        Assert.That(res, Is.Not.Null);
        Assert.Multiple(() => { Assert.That(res!.testOverwriteProperty, Is.EqualTo("overwritten")); });
    }

    [Test]
    public void printsHelp() {
        var writer = new StringWriter();
        ConfigReader.printHelp<TestConfig>(writer);
        Assert.That(writer.ToString(), Is.EqualTo("Usage: ArgsConfigReaderTest -[pibo]\nOptional flags:\nShort   Full          Default       Description\n-p      --property    default       A property\n-i      --int         12            An int property\n-b      --bool        false         \n-o      --overwrite   def           An overwriteable property\n-w                    overwritten   \n"));
    }
}