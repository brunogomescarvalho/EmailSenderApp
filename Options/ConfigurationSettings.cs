namespace EmailSenderApp.Options;

public class ConfigurationSettings
{
    public string EmailSenderAddress { get; }
    public string EmailSenderCredential { get; }

    public ConfigurationSettings()
    {
        EmailSenderAddress = Environment.GetEnvironmentVariable("EMAIL_SENDER_ADDRESS")
            ?? throw new ArgumentNullException(nameof(EmailSenderAddress));

        EmailSenderCredential = Environment.GetEnvironmentVariable("EMAIL_SENDER_CREDENTIAL")
            ?? throw new ArgumentNullException(nameof(EmailSenderCredential));
    }
}
