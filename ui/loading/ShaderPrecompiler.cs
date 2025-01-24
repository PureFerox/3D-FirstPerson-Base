using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public partial class ShaderPrecompiler : Node
{
	private const string CONFIG_FILE_PATH = "res://config/precompile_list.cfg";

	private HashSet<string> _scenesToCompile;
	private HashSet<string> _compiledMaterialPaths;
	private int currentLoadIndex = 0;
	private int compilingMaterialsCount = 0;

	[Export] private bool _doScenePathCaching = false;
	[Export] private Node3D _compileNode3D;
	[Export] private Texture2D _defaultTexture;

	[Signal] public delegate void ScanFilesCompleteEventHandler();
	[Signal] public delegate void ShaderPrecompilationCompleteEventHandler();

	public override void _Ready()
	{
		GetTree().CreateTimer(1f).Timeout += () => ScanFiles();
		_scenesToCompile = new HashSet<string>();
		_compiledMaterialPaths = new HashSet<string>();
	}

	public override void _Process(double delta)
	{
	}

	private void ScanFiles() {
		if (OS.IsDebugBuild() && _doScenePathCaching) {
			ParseScenePathsToCompile();
		}

		using FileAccess file = FileAccess.Open(CONFIG_FILE_PATH, FileAccess.ModeFlags.Read);
		string path = file.GetLine();
		while (!string.IsNullOrEmpty(path)) {
			_scenesToCompile.Add(path);
			path = file.GetLine();
		}

		EmitSignal(nameof(ScanFilesComplete));
	}

	private void ParseScenePathsToCompile() {
		List<string> scenePathList = new List<string>();

		string regexPattern = @"(sub_resource|ext_resource) type=""(ParticalProcessMaterial|ShaderMaterial|Material|CanvasItemMaterial)""";
		Regex regex = new Regex(regexPattern, RegexOptions.Multiline);

		List<string> fileList = new List<string>();
		new FileSystem().ForFilesInDirectory("res://", (_, filepath) => fileList.Add(filepath), true);

		List<string> instantiationList = new List<string>();

		foreach (string fPath in fileList) {
			using FileAccess file = FileAccess.Open(fPath, FileAccess.ModeFlags.Read);

			if (file == null)
				continue;
			
			string text;
			try {
				byte[] fileBytes = file.GetBuffer((int)file.GetLength());
				text = new System.Text.UTF8Encoding(false, false).GetString(fileBytes);
			}
			catch (System.Exception ex) {
				GD.PrintErr($"Error reading file '{fPath}': {ex.Message}");
				continue;
			}
			Match match = regex.Match(text);

			if (match.Success)
				instantiationList.Add(fPath);
		}

		FileAccess saveFile = FileAccess.Open(CONFIG_FILE_PATH, FileAccess.ModeFlags.Write);
		foreach (string path in instantiationList) {
			saveFile.StoreLine(path);
		}
		saveFile.Close();
	}

	private async void OnScanFilesComplete() {
		// Compile All Materials
		int count = 0;
		foreach (string scenePath in _scenesToCompile) {
			await CompileMaterialAsync(scenePath);
			count++;
			_scenesToCompile.Remove(scenePath);
		}

		if (_scenesToCompile.Count == 0 && compilingMaterialsCount == 0) {
			EmitSignal(nameof(ShaderPrecompilationComplete));
		}
	}

	private async Task CompileMaterialAsync(string scenePath) {
		Node scene = ResourceLoader.Load<PackedScene>(scenePath).Instantiate<Node>();

		if (scene is GpuParticles3D p)
			await CompileParticlesNodeAsync(p);
		
		if (scene is MeshInstance3D m)
			await CompileMeshNodeAsync(m);

		List<Node> descendents = GetAllDescendants(scene);

		foreach (GpuParticles3D d in descendents.OfType<GpuParticles3D>())
			await CompileParticlesNodeAsync(d);
		
		foreach (MeshInstance3D d in descendents.OfType<MeshInstance3D>()) {
			await CompileMeshNodeAsync(d);
		}

		scene.QueueFree();
	}

	private async Task CompileParticlesNodeAsync(GpuParticles3D particles3D) {
		await Task.Delay(500);
	}

	private async Task CompileMeshNodeAsync(MeshInstance3D meshInstance3D) {
		if (meshInstance3D.Mesh != null) {
			if (meshInstance3D.Mesh.GetSurfaceCount() > 0) {
				for (int i = 0; i < meshInstance3D.Mesh.GetSurfaceCount(); i++) {
					
					if (meshInstance3D.Mesh.SurfaceGetMaterial(i) != null) {
						Material material = meshInstance3D.Mesh.SurfaceGetMaterial(i);

						if (!string.IsNullOrEmpty(material.ResourcePath) && !_compiledMaterialPaths.Contains(material.ResourcePath)) {
							compilingMaterialsCount++;
							_compiledMaterialPaths.Add(material.ResourcePath);

							Sprite3D sprite3D = new Sprite3D() {
								Texture = _defaultTexture,
								MaterialOverride = material
							};
							_compileNode3D.AddChild(sprite3D);

							await Task.Delay(500);
							
							compilingMaterialsCount--;
							if (IsInstanceValid(sprite3D)) {
								sprite3D.QueueFree();
							}
						}
					}
				}
			}
		}
	}

	public List<Node> GetAllDescendants(Node node) {
		List<Node> list = new List<Node>();

		if (node.GetChildren().Count > 0) {
			foreach (Node child in node.GetChildren()) {
				list.AddRange(GetAllDescendants(child));
				list.Add(child);
			}
			return list;
		}
		return new List<Node>();
	}
}
