using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.V1.Entity
{
    public class Menus
    {
        public Menus()
        {
            this.MenuSet1 = new List<Menu>();
            this.MenuSet2 = new List<Menu>();
            this.ProfileMenus = new List<Menu>();
            this.HideMenus = new List<Menu>();
        }
        public Menus(string menuId, SharedMenus shareMenus) : this()
        {
            this.MenuId = menuId;
            this.ProfileMenus = shareMenus.ProfileMenus;
            this.HideMenus = shareMenus.HideMenus;
        }
        public IList<Menu> MenuSet1 { get; set; }
        public IList<Menu> MenuSet2 { get; set; }
        public IList<Menu> ProfileMenus { get; set; }
        public IList<Menu> HideMenus { get; set; }
        public string MenuId { get; set; }
        public string MenuTitle { 
            get
            {
                string mtitle = string.Empty;
                mtitle = this.MenuSet1.Where(p => p.MenuId.ToLower() == this.MenuId.ToLower()).Select(p => p.Title).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(mtitle) == false) return mtitle;

                foreach(Menu root1 in this.MenuSet1)
                {
                    mtitle = root1.Nodes.Where(p => p.MenuId.ToLower() == this.MenuId.ToLower()).Select(p => p.Title).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(mtitle)==false) return mtitle;
                }

                mtitle = this.MenuSet2.Where(p => p.MenuId.ToLower() == this.MenuId.ToLower()).Select(p => p.Title).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(mtitle) == false) return mtitle;

                foreach (Menu root2 in this.MenuSet2)
                {
                    mtitle = root2.Nodes.Where(p => p.MenuId.ToLower() == this.MenuId.ToLower()).Select(p => p.Title).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(mtitle) == false) return mtitle;
                }

                mtitle = this.ProfileMenus.Where(p => p.MenuId.ToLower() == this.MenuId.ToLower()).Select(p => p.Title).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(mtitle) == false) return mtitle;

                mtitle = this.HideMenus.Where(p => p.MenuId.ToLower() == this.MenuId.ToLower()).Select(p => p.Title).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(mtitle) == false) return mtitle;

                return mtitle;
            }
        }
        public void AddSet1(Menu menu)
        {
            this.MenuSet1.Add(menu);
        }
        public void AddSet2(Menu menu)
        {
            this.MenuSet2.Add(menu);
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
        public string Indent { get; set; }
        public string MenuImage { get; set; }
        public string Sort { get; set; }
        public IList<Menu> Nodes { get; set; }
    }
    public abstract class SharedMenus
    {
        public SharedMenus()
        {
            this.ProfileMenus = new List<Menu>();
            this.HideMenus = new List<Menu>();
        }
        public List<Menu> ProfileMenus { get; set; }
        public List<Menu> HideMenus { get; set; }
        public List<string> GetSharedMenus()
        {
            List<string> pubMenus = new List<string>();
            foreach (Menu m1 in this.HideMenus) pubMenus.Add(m1.MenuId);
            foreach (Menu m1 in this.ProfileMenus) pubMenus.Add(m1.MenuId);
            return pubMenus;
        }
    }
    public class AdminSharedMenus : SharedMenus
    {
        public AdminSharedMenus() : base()
        {
            this.ProfileMenus = new List<Menu>();
            this.HideMenus = new List<Menu>();
            InitProfile();
            InitHide();
        }
        public void InitProfile()
        {
            this.ProfileMenus.Add(new Menu { MenuId = "P01", Title = LanguageHelper.Words("my.account"),          Url = "/Admin/Profile/MyAccount" });
            this.ProfileMenus.Add(new Menu { MenuId = "P02", Title = LanguageHelper.Words("reset.password"),      Url = "/Admin/Profile/ResetPassword" });
            this.ProfileMenus.Add(new Menu { MenuId = "P03", Title = LanguageHelper.Words("my.message"),          Url = "/Admin/Profile/MyMessage" });
        }
        public void InitHide()
        {
            this.HideMenus.Add(new Menu { MenuId = "Login", Title = LanguageHelper.Words("Login"), Url = "/Admin/Home/Index" });
        }
    }

    public class PubSharedMenus : SharedMenus
    {
        public PubSharedMenus() : base()
        {
            this.ProfileMenus = new List<Menu>();
            this.HideMenus = new List<Menu>();
            InitProfile();
            InitHide();
        }
        public void InitProfile()
        {
            this.ProfileMenus.Add(new Menu { MenuId = "P01", Title = LanguageHelper.Words("my.account"),      Url = "/Profile/MyAccount" });
            this.ProfileMenus.Add(new Menu { MenuId = "P02", Title = LanguageHelper.Words("my.message"),      Url = "/Profile/MyMessage" });
            this.ProfileMenus.Add(new Menu { MenuId = "P03", Title = LanguageHelper.Words("logout"),          Url = "/Profile/SignOut" });
        }
        public void InitHide()
        {
            this.HideMenus.Add(new Menu { MenuId = "SignIn", Title = LanguageHelper.Words("SignIn"),  Url = "/Home/SignIn" });
        }
    }

}
