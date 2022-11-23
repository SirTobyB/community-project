using Cysharp.Threading.Tasks;

namespace BoundfoxStudios.CommunityProject.Infrastructure.SaveManagement
{
	public interface IHaveSaveData<T>
	{
		UniTask<T> GetDataContainerAsync();
		UniTask SetDataContainerAsync(T container);
	}
}
