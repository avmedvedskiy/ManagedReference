# ManagedReference
Managed Reference
The `SerializeReference` attribute, added in Unity 2019.3, makes it possible to serialize references to interfaces and abstract classes.

The `ManagedReference` attribute allows you to easily set subclasses of those abstract classes in the Editor that are serialized by `SerializeReference` attribute.

![image](https://user-images.githubusercontent.com/17832838/142038888-38576c65-41e5-4c00-b5f4-ecd7522af5ec.png)


#### Features
 - Set a subclasses from dropdown
 - Group classes by the `ManagedReferenceGroupAttribute` with custom groupname
 - Create arrays of subclasses

#### Install via git URL

`https://github.com/avmedvedskiy/ManagedReference.git?path=Assets/ManagedReference`

from the `Add package from git URL` option.

# Action Nodes
Actions based on the `ManagedReference` attribute and `IAction` interface allow you to run a sequence of a custom actions.

![image](https://user-images.githubusercontent.com/17832838/142039615-e25db621-9360-4155-a66c-afffa5546291.png)


#### Features
 - Custom actions with async Run method
 - Custom ActionRunner support 
 - Support Nested Sequences

#### Install via git URL

`https://github.com/avmedvedskiy/ManagedReference.git?path=Assets/ActionNodes`

from the `Add package from git URL` option.

## 🔰 Usage

```cs
  public List<IAction> actions;
  async void RunAsync()
  {
      ActionRunner runner = new ActionRunner();
      await runner.RunAsync(actions);
      Debug.Log("Finish");
  }
```
