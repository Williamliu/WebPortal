using System;
using System.Collections.Generic;
using System.Text;

namespace Library.V1.Entity
{
    public class Menus
    {
        public Menus()
        {
            this.TopMenu = new List<Menu>();
            this.BottomMenu = new List<Menu>();
            this.ProfileMenus = new List<Menu>();
            this.HideMenus = new List<Menu>();
        }
        public Menus(string menuId, PubMenus pubMenus) : this()
        {
            this.MenuId = menuId;
            this.ProfileMenus = pubMenus.ProfileMenus;
            this.HideMenus = pubMenus.HideMenus;
        }
        public IList<Menu> TopMenu { get; set; }
        public IList<Menu> BottomMenu { get; set; }
        public IList<Menu> ProfileMenus { get; set; }
        public IList<Menu> HideMenus { get; set; }
        public string MenuId { get; set; }
        public string MenuTitle { get; set; }
        public void AddTop(Menu menu)
        {
            this.TopMenu.Add(menu);
        }
        public void AddBottom(Menu menu)
        {
            this.BottomMenu.Add(menu);
        }
    }
    public class Menu
    {
        public Menu()
        {
            this.Nodes = new List<Menu>();
            this.Guid = Guid.NewGuid();
            this.State = EState.Normal;
        }
        public Guid Guid { get; set; }
        public EState State { get; set; }
        public int Id { get; set; }
        public string ParentId { get; set; }
        public string MenuId { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Url { get; set; }
        public string Position { get; set; }
        public string MenuImage { get; set; }
        public string Sort { get; set; }
        public IList<Menu> Nodes { get; set; }
    }
    public abstract class PubMenus
    {
        public PubMenus()
        {
            this.ProfileMenus = new List<Menu>();
            this.HideMenus = new List<Menu>();
        }
        public List<Menu> ProfileMenus { get; set; }
        public List<Menu> HideMenus { get; set; }
    }
    public class AdminPubMenus : PubMenus
    {
        public AdminPubMenus() : base()
        {
            this.ProfileMenus = new List<Menu>();
            this.HideMenus = new List<Menu>();
            InitProfile();
            InitHide();
        }
        public void InitProfile()
        {
            this.ProfileMenus.Add(new Menu { MenuId = "P01", Title = "my.account", Detail = "my.account", Url = "/Admin/Profile/MyAccount" });
            this.ProfileMenus.Add(new Menu { MenuId = "P02", Title = "reset.password", Detail = "reset.password", Url = "/Admin/Profile/ResetPassword" });
            this.ProfileMenus.Add(new Menu { MenuId = "P03", Title = "my.message", Detail = "my.account", Url = "/Admin/Profile/MyMessage" });
        }
        public void InitHide()
        {
            this.HideMenus.Add(new Menu { MenuId = "Login", Title = "Login", Url = "/Admin/Home/Index" });
        }
        public List<string> GetPublicMenus()
        {
            List<string> pubMenus = new List<string>();
            foreach (Menu m1 in this.HideMenus) pubMenus.Add(m1.MenuId);
            foreach (Menu m1 in this.ProfileMenus) pubMenus.Add(m1.MenuId);
            return pubMenus;
        }
    }
}
