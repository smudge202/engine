﻿using Components;
using FluentAssertions;
using Microsoft.Framework.DependencyInjection;
using System;
using Xunit;

namespace Component.Tests
{
	public class ComponentTests
	{
		[Fact]
		public static void WhenServiceAddedThenCanResolveComponentSystem()
		{
			new ServiceCollection()
				.AddComponentSystem()
				.BuildServiceProvider()
				.GetService<ComponentSystem>()
				.Should().NotBeNull();
		}

		private static IServiceProvider CreateProvider()
			=> new ServiceCollection()
				.AddComponentSystem()
				.BuildServiceProvider();

		private static ComponentSystem CreateTarget(IServiceProvider provider = null)
			=> (provider ?? CreateProvider())
				.GetRequiredService<ComponentSystem>();

		[Fact]
		public static void WhenAssigningComponentToNullEntityThenReturnsFalse()
		{
			CreateTarget().Assign((object)null, new object())
				.Should().BeFalse();
		}

		[Fact]
		public static void WhenAssigningNullComponentToEntityThenReturnsFalse()
		{
			CreateTarget().Assign(new object(), (object)null)
				.Should().BeFalse();
		}

		[Fact]
		public static void WhenAssigningComponentToEntityThenReturnsTrue()
		{
			CreateTarget().Assign(new object(), new object())
				.Should().BeTrue();
		}

		[Fact]
		public static void WhenAssigningSameComponentTypeToSameEntityInstanceThenReturnsFalse()
		{
			var entity = new object();
			var provider = CreateProvider();

			CreateTarget(provider).Assign(entity, new object())
				.Should().BeTrue();
			CreateTarget(provider).Assign(entity, new object())
				.Should().BeFalse();
		}

		[Fact]
		public static void WhenAssigningSameComponentsToDifferentEntityInstancesThenReturnsTrue()
		{
			var provider = CreateProvider();
			var component = new object();
			CreateTarget(provider).Assign(new object(), component)
				.Should().BeTrue();
			CreateTarget(provider).Assign(new object(), component)
				.Should().BeTrue();
		}

		[Fact]
		public static void WhenRetrievingStateForEntityNeverAssignedToThenDoesNotThrowException()
		{
			Action act = () => CreateTarget().Get(new object());
			act.ShouldNotThrow();
		}

		[Fact]
		public static void WhenRetrievingEntityStateForEntityNeverAssignedToThenReturnsNull()
		{
			CreateTarget().Get(new object())
				.Should().BeNull();
		}

		[Fact]
		public static void WhenRetrievingComponentAssignedToSameEntityInstanceThenReturnsComponent()
		{
			var entity = new object();
			var component = new object();
			var target = CreateTarget();
			target.Assign(entity, component);

			target.Get(entity).Get<object>()
				.Should().Be(component);
		}

		[Fact]
		public static void WhenRetrievingComponentAssignedToDifferentEntityInstanceThenReturnsDefaultComponent()
		{
			var target = CreateTarget();
			target.Assign(new object(), new object());

			target.Get(new object())?.Get<object>()
				.Should().Be(default(object));
		}

		[Fact]
		public static void WhenUnassigningComponentFromNullEntityThenReturnsFalse()
		{
			CreateTarget().Unassign((object)null, new object())
				.Should().BeFalse();
		}

		[Fact]
		public static void WhenUnassigningComponentNeverAssignedToEntityThenReturnsFalse()
		{
			CreateTarget().Unassign(new object(), new object())
				.Should().BeFalse();
		}

		[Fact]
		public static void WhenUnassigningComponentOfSameTypeAsAssignedComponentThenReturnsTrue()
		{
			var entity = new object();
			var provider = CreateProvider();
			CreateTarget(provider).Assign(entity, new object());

			CreateTarget(provider).Unassign(entity, new object())
				.Should().BeTrue();
		}

		[Fact]
		public static void WhenUnassigningUnassignedComponentThenReturnsFalse()
		{
			var entity = new object();
			var provider = CreateProvider();
			CreateTarget(provider).Assign(entity, new object());

			CreateTarget(provider).Unassign(entity, new object())
				.Should().BeTrue();
			CreateTarget(provider).Unassign(entity, new object())
				.Should().BeFalse();
		}
	}
}