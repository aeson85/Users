using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Users.Infrastructure.TagHelpers
{
    [HtmlTargetElement("td", Attributes="identity-claim-type")]
    public class ClaimTypeTagHelper : TagHelper
    {
        [HtmlAttributeName("identity-claim-type")]
        public string ClaimType { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var foundType = false;
            var fields = typeof(ClaimTypes).GetFields();
            foreach (var field in fields)
            {
                if (field.GetValue(null).ToString() == this.ClaimType)
                {
                    output.Content.SetContent(field.Name);
                    foundType = true;
                }
                if (!foundType)
                {
                    output.Content.SetContent(this.ClaimType.Split('/', '.').Last());
                }
            }
        }
    }
}