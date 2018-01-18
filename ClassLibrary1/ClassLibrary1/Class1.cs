using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluent.API
{
    public class ConditionActionRelation
    {
        public Action ThenDoes { get; set; }

        public Action ElseDoes { get; set; }

        public Func<bool> When { get; set; }
    }

    public class WhenCondition : conditionBase
    {
        public OrAndCondition When(Func<bool> condition)
        {
            Next = new OrAndCondition { Does = condition };

            return (OrAndCondition)Next;
        }

        public ElseWhenCondition When(Func<bool> condition, Action action)
        {
            return new ThenElseResult().Then(condition, action);
        }
    }

    public abstract class conditionBase
    {
        public conditionBase Next { get; protected set; }

        public Func<bool> Does { get; set; }

        protected virtual void SetNext(conditionBase condition)
        {
            Next = condition;
        }
    }

    public class OrAndCondition : conditionBase
    {
        public OrAndCondition Or(Func<bool> condition)
        {
            Next = new OrAndCondition { Does = condition };

            return (OrAndCondition)Next;
        }

        public OrAndCondition And(Func<bool> condition)
        {
            Next = new OrAndCondition { Does = condition };

            return (OrAndCondition)Next;
        }

        public void Then(Action action)
        {
            new ThenElseResult().Then(action);
        }

        public ElseWhenCondition Then(Func<bool> condition, Action action)
        {
            return new ThenElseResult().Then(condition, action);
        }
    }

    public class AndCondition
    {
    }

    public class ElseWhenCondition
    {
        public Action Does { get; private set; }
        public Func<bool> Condition { get; private set; }

        public void Else(Action action)
        {
            Does = action;
        }

        public ThenElseResult ElseWhen(Func<bool> condition, Action action)
        {
            Does = action;

            Condition = condition;

            return new ThenElseResult() { Condition = condition, Does = action };
        }
    }

    public class ThenElseResult
    {
        public Func<bool> Condition { get; set; }
        public Action Does { get; set; }

        public void Then(Action action)
        {
            Does = action;
        }

        public ElseWhenCondition Then(Func<bool> condition, Action action)
        {
            Condition = condition;

            Does = action;

            return new ElseWhenCondition();
        }
    }

    public class ElseElseWhen
    {
        public ElseWhenCondition
    }

    public class ConditionHandler
    {
        public void Apply(Func<bool> condition, Action then, Action elseAct)
        {
            WhenCondition test = null;

            test.When(condition)
                .Or(condition)
                .And(condition)
                .Or(condition)
                .Then(() => true, () => then())
                .ElseWhen(condition, then)
                .Then(condition, then)
                .ElseWhen(condition, then)
                .Then(then);

            if (condition())
            {
                then?.Invoke();
            }
            else
            {
                elseAct?.Invoke();
            }
        }

        public void Apply(IEnumerable<ConditionActionRelation> conditions)
        {
            foreach (var c in conditions)
            {
                Apply(c.When, c.ThenDoes, c.ElseDoes);
            }
        }
    }
}