using System.Linq;
using Verse;

namespace BetterPawnControl
{

    public class Dialog_RenamePolicy : Dialog_Rename
    {

        private Policy policy = null;
        private int maxLength = 12;
        private Resources.Type type = Resources.Type.animal;

        public Dialog_RenamePolicy(Policy policy, Resources.Type type)
        {
            this.policy = policy;
            this.curName = policy.label;
            this.type = type;
        }

        /// <summary>
        /// Copy paste from vanilla
        /// </summary>
        protected override AcceptanceReport NameIsValid(string name)
        {
            AcceptanceReport result = base.NameIsValid(name);
            if (!result.Accepted)
            {
                return result;
            }

            string str = null;

            switch (this.type)
            {
                case Resources.Type.animal:
                    if (AnimalManager.policies.Any((Policy d) => d.label == name))
                    {
                        str = "NameIsInUse".Translate();
                    }
                    break;
                case Resources.Type.assign:
                    if (AssignManager.policies.Any((Policy d) => d.label == name))
                    {
                        str = "NameIsInUse".Translate();
                    }
                    break;
            }
            if (!str.NullOrEmpty())
            {
                return str;
            }
            return true;
        }

        /// <summary>
        /// Sanitize AnimalPolicy label name and set refresh label on (grand)parent window
        /// </summary>
        protected override void SetName(string name)
        {
            this.policy.label = this.curName.Length <= maxLength ? this.curName : this.curName.Substring(0, maxLength) + "...";
        }
    }
}
