using UnityEngine;

public class ExUITStyle {
    private ExUITModel model;

    public ExUITStyle(ExUITModel model) {
        this.model = model;
    }

    public ExUITComponent GetStyleComponent(TypeComponents? type) {
        if (type.HasValue) {
            for (int i = 0; i < model.components.Length; i++) {
                if (model.components[i].type == type.Value) {
                    return model.components[i];
                }
            }
        }
        return null;
    }

    public void Update(ExUITModel newModel) {
        this.model = newModel;
    }

    public ExUITModel GetModel() {
        
        if (model != null)
        {
            return model;
        }

        return null;
    }
}
