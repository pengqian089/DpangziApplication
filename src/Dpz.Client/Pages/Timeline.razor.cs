using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dpz.Client.Data;
using Dpz.Client.Models;

namespace Dpz.Client.Pages
{
    public partial class Timeline
    {
        private bool _isLoading = false;

        private double _width = 0d;

        private DotNetObjectReference<Timeline> _objectReference;

        [Inject] private IJSRuntime JsRuntime { get; set; }

        [Inject]private TimelineService TimelineService { get; set; }

        private List<TimelineModel> _source = new();

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            _width = await JsRuntime.InvokeAsync<double>("getWindowWidth");
            _objectReference = DotNetObjectReference.Create(this);
            _source = await TimelineService.GetTimelineAsync();
            _isLoading = false;
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitWindowWidthListener();
            }
        }

        private async Task InitWindowWidthListener()
        {
            await JsRuntime.InvokeVoidAsync("addWindowWidthListener", _objectReference);
        }

        public async ValueTask DisposeAsync()
        {
            await JsRuntime.InvokeVoidAsync("removeWindowWidthListener", _objectReference);
            _objectReference?.Dispose();
        }

        [JSInvokable]
        public void UpdateWindowWidth(int windowWidth)
        {
            _width = windowWidth;
            StateHasChanged();
        }
    }
}
