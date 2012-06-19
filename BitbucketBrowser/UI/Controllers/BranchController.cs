using System;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Linq;


namespace BitbucketBrowser.UI
{
	public class BranchController : Controller<List<BranchModel>>
	{
        public string Username { get; private set; }

        public string Slug { get; private set; }

		public BranchController(string username, string slug) 
            : base(true, true)
		{
			Style = UITableViewStyle.Plain;
            Username = username;
            Slug = slug;
            Title = "Branches";
            Root.Add(new Section());
		}
		
        protected override void OnRefresh()
        {
            InvokeOnMainThread(delegate {
                Root[0].Clear();
                Root[0].AddAll(from x in Model 
                               select (Element)new StyledStringElement(x.Branch, () => NavigationController.PushViewController(new SourceController(Username, Slug, x.Branch), true))
                               { Accessory = UITableViewCellAccessory.DisclosureIndicator });
            });
        }

        protected override List<BranchModel> OnUpdate()
        {
            var client = new BitbucketSharp.Client("thedillonb", "djames");
            var b = new List<BranchModel>(client.Users[Username].Repositories[Slug].Branches.GetBranches().Values);
            b.Sort((x,y) => {
                if (x.Branch == "master")
                    return 1;
                return x.Branch.CompareTo(y.Branch); 
            });
            return b;
        }
	}
}
