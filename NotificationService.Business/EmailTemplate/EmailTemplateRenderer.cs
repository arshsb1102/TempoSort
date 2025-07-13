using Scriban;

public class EmailTemplateRenderer
{
    public string RenderTemplate(string filePath, object model)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Template file not found", filePath);

        var templateText = File.ReadAllText(filePath);
        var template = Template.Parse(templateText);

        if (template.HasErrors)
            throw new InvalidOperationException($"Template parsing error: {string.Join(", ", template.Messages)}");

        return template.Render(model, member => member.Name);
    }
}
