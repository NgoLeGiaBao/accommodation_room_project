using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace App.Menu
{
    public enum SideBarItemType
    {
        Divider,
        Heading,
        NavItem
    }
    public class SideBarItem
    {
        public string Title { get; set; }
        public bool isActive { get; set; }
        public SideBarItemType Type { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Area { get; set; }
        public string AwesomeIcon { get; set; }
        public string CollapseId { get; set; }
        public int? ParentId { get; set; }
        public virtual SideBarItem Parent { get; set; }
        public virtual List<SideBarItem> Items { get; set; }

        public string GetLink(IUrlHelper urlHelper)
        {
            return urlHelper.Action(Action, Controller, new { area = Area });
        }

        public string RenderHtml(IUrlHelper urlHelper)
        {
            var html = new StringBuilder();
            if (Type == SideBarItemType.Heading)
            {
                html.Append($@"
                    <div class=""sidebar-heading"">
                        {Title}
                    </div>
                ");
            }
            else if (Type == SideBarItemType.NavItem)
            {
                if (Items == null)
                {
                    var url = GetLink(urlHelper);
                    var icon = (AwesomeIcon != null) ? AwesomeIcon : "";
                    var cssClass = "nav-item";
                    if (isActive)
                    {
                        cssClass += " active";
                    }

                    html.Append($@"
                        <li class=""{cssClass}"">
                            <a class=""nav-link"" href=""{url}"">
                                <i class=""{icon}""></i>
                                <span>{Title}</span>
                            </a>
                        </li>
                    ");
                }
                else
                {
                    var cssClass = "nav-item";
                    var cssCollapse = "collapse";
                    var icon = AwesomeIcon ?? ""; // Tối ưu toán tử null

                    if (isActive)
                    {
                        cssClass += " active";
                    }


                    var itemMenu = "";
                    foreach (var item in Items)
                    {
                        var urlItem = item.GetLink(urlHelper);
                        var cssItem = "collapse-item";
                        if (item.isActive)
                        {
                            cssItem += " active";
                        }
                        itemMenu += $@"<a class=""{cssItem}"" href=""{urlItem}"">{item.Title}</a>";
                    }

                    html.Append($@"
                        <li class=""{cssClass}"">
                            <a class=""nav-link collapsed"" href=""#"" data-toggle=""collapse"" data-target=""#{CollapseId}""
                               aria-expanded=""{(isActive ? "true" : "false")}"" aria-controls=""{CollapseId}"">
                                <i class=""{icon}""></i>
                                <span>{Title}</span>
                            </a>
                            <div id=""{CollapseId}"" class=""{cssCollapse}"" aria-labelledby=""headingTwo"" data-parent=""#accordionSidebar"">
                                <div class=""bg-white py-2 collapse-inner rounded"">
                                    {itemMenu}
                                </div>
                            </div>
                        </li>
                    ");
                }
            }
            else if (Type == SideBarItemType.Divider)
            {
                html.Append("<hr class=\"sidebar-divider d-none d-md-block\">");

            }

            return html.ToString();
        }
    }
}