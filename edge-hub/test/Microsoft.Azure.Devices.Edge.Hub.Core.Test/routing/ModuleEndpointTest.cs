﻿// Copyright (c) Microsoft. All rights reserved.
namespace Microsoft.Azure.Devices.Edge.Hub.Core.Test.Routing
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Devices.Edge.Hub.Core.Device;
    using Microsoft.Azure.Devices.Edge.Hub.Core.Routing;
    using Microsoft.Azure.Devices.Edge.Util;
    using Microsoft.Azure.Devices.Edge.Util.Test.Common;
    using Microsoft.Azure.Devices.Routing.Core;
    using Moq;
    using Xunit;
    using IRoutingMessage = Microsoft.Azure.Devices.Routing.Core.IMessage;

    [Unit]
    public class ModuleEndpointTest
    {
        [Fact]
        public void ModuleEndpoint_MembersTest()
        {
            Core.IMessageConverter<IRoutingMessage> routingMessageConverter = new RoutingMessageConverter();
            var deviceProxyMock = new Mock<IDeviceProxy>();
            string moduleId = "device1/module1";
            string moduleEndpointAddress = "in1";

            var moduleEndpoint = new ModuleEndpoint(moduleId, moduleEndpointAddress, () => Option.Some(deviceProxyMock.Object), routingMessageConverter);

            Assert.Equal(moduleId, moduleEndpoint.Id);
            Assert.Equal("ModuleEndpoint", moduleEndpoint.Type);
            Assert.Equal(moduleId, moduleEndpoint.Name);
            Assert.Equal(string.Empty, moduleEndpoint.IotHubName);
        }

        [Fact]
        public void ModuleMessageProcessor_CreateProcessorTest()
        {
            Core.IMessageConverter<IRoutingMessage> routingMessageConverter = new RoutingMessageConverter();
            var deviceProxyMock = new Mock<IDeviceProxy>();
            string moduleId = "device1/module1";
            string moduleEndpointAddress = "in1";

            var moduleEndpoint = new ModuleEndpoint(moduleId, moduleEndpointAddress, () => Option.Some(deviceProxyMock.Object), routingMessageConverter);

            IProcessor moduleMessageProcessor = moduleEndpoint.CreateProcessor();
            Assert.NotNull(moduleMessageProcessor);
            Assert.Equal(moduleEndpoint, moduleMessageProcessor.Endpoint);
        }

        [Fact]
        public void ModuleMessageProcessor_CloseAsyncTest()
        {
            Core.IMessageConverter<IRoutingMessage> routingMessageConverter = new RoutingMessageConverter();
            var deviceProxyMock = new Mock<IDeviceProxy>();
            string moduleId = "device1/module1";
            string moduleEndpointAddress = "in1";

            var moduleEndpoint = new ModuleEndpoint(moduleId, moduleEndpointAddress, () => Option.Some(deviceProxyMock.Object), routingMessageConverter);

            IProcessor moduleMessageProcessor = moduleEndpoint.CreateProcessor();
            Task result = moduleMessageProcessor.CloseAsync(CancellationToken.None);
            Assert.Equal(TaskEx.Done, result);
        }

        [Fact]
        public async Task ModuleMessageProcessor_ProcessAsyncTest()
        {
            Core.IMessageConverter<IRoutingMessage> routingMessageConverter = new RoutingMessageConverter();
            var deviceProxy = new Mock<IDeviceProxy>();
            var routingMessage = Mock.Of<IRoutingMessage>();
            string moduleId = "device1/module1";
            string moduleEndpointAddress = "in1";

            var moduleEndpoint = new ModuleEndpoint(moduleId, moduleEndpointAddress, () => Option.Some(deviceProxy.Object), routingMessageConverter);

            IProcessor moduleMessageProcessor = moduleEndpoint.CreateProcessor();
            ISinkResult<IMessage> sinkResult = await moduleMessageProcessor.ProcessAsync(routingMessage ,CancellationToken.None);
            Assert.True(sinkResult.Failed.Contains(routingMessage));
        }

        [Fact]
        public async Task Events_NoDeviceProxyTest()
        {
            Core.IMessageConverter<IRoutingMessage> routingMessageConverter = new RoutingMessageConverter();
            var routingMessage = Mock.Of<IRoutingMessage>();
            string moduleId = "device1/module1";
            string moduleEndpointAddress = "in1";

            var moduleEndpoint = new ModuleEndpoint(moduleId, moduleEndpointAddress, () => Option.None<IDeviceProxy>(), routingMessageConverter);

            IProcessor moduleMessageProcessor = moduleEndpoint.CreateProcessor();
            ISinkResult<IMessage> sinkResult = await moduleMessageProcessor.ProcessAsync(routingMessage, CancellationToken.None);
            Assert.Equal(FailureKind.InternalError, sinkResult.SendFailureDetails.Map(x => x.FailureKind).GetOrElse(FailureKind.None));
        }
    }
}