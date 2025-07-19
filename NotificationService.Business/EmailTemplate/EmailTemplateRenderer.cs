using Scriban;

public class EmailTemplateRenderer
{
    private readonly string _baseTemplateDir;

    public EmailTemplateRenderer()
    {
        _baseTemplateDir = Path.Combine(AppContext.BaseDirectory, "EmailTemplate", "Templates", "Email");
    }

    public string RenderTemplate(string relativeTemplateName, object model)
    {
        // Automatically build full path
        var filePath = Path.Combine(_baseTemplateDir, relativeTemplateName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException("Template file not found", filePath);

        var templateText = File.ReadAllText(filePath);
        var template = Template.Parse(templateText);

        if (template.HasErrors)
            throw new InvalidOperationException($"Template parsing error: {string.Join(", ", template.Messages)}");

        return template.Render(model, member => member.Name);
    }
}
