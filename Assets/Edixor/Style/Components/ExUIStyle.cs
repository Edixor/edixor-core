using UnityEngine;

public class BFPStyle {
    private BFPStyleModel model;

    public BFPStyle(BFPStyleModel model) {
        this.model = model;
    }

    public BFPStyleComponent GetStyleComponent(string name, TypeComponents? type = null) {
        if (type.HasValue) {
            for (int i = 0; i < model.components.Length; i++) {
                if (model.components[i].type == type.Value) {
                    return model.components[i];
                }
            }
        }

        for (int i = 0; i < model.components.Length; i++) {
            if (model.components[i].name == name) {
                return model.components[i];
            }
        }
        return null;
    }

    public void Update(BFPStyleModel newModel) {
        this.model = newModel;
    }

    public BFPStyleModel GetModel() {
        
        if (model != null)
        {
            return model;
        }

        return null;
    }
}
