<script lang="ts">
  import { T } from '@threlte/core';
  import { GLTF } from '@threlte/extras';
  import { useGltfAnimations } from './useGltfAnimations.js';
  import ScrollCamera from './scrollCamera.svelte';
  import { Text } from '@threlte/extras';

  const ANIMATION_NAME = "MainAnim";
  const SCROLL_ANIM_AMOUNT = 0.005;

  const { scrollY } = $props(); 

  const TEXT_LINES = [
    "Hello!",
    "This is testing a scroll effect",
    "using Svelte + Threlte!",
    "",
    "It's a bit scuffed",
    "but it kinda works",
    "If I were to redo this, I would",
    "pick a more appropriate thing",
    "to use"
  ];
  

  let animationTime = $state(0);
  let showText = $derived(scrollY * SCROLL_ANIM_AMOUNT >= animationTime);

  const {gltf, actions, mixer} = useGltfAnimations<typeof ANIMATION_NAME>();

  $effect(() => {
    $actions[ANIMATION_NAME]?.play();
    animationTime = $actions[ANIMATION_NAME]?.getClip()?.duration ?? 0;
  });
  
  $effect(() => {
    mixer.setTime(Math.min(scrollY * SCROLL_ANIM_AMOUNT, animationTime - 0.01));
  });

</script>

<ScrollCamera />

<T.DirectionalLight
  intensity={0.6}
  position={[2, -10, -0.5]}
  castShadow
  shadow.bias={-0.0001}
/>

<GLTF url="/page.glb"  bind:gltf={$gltf} />

{#if showText}
  {#each TEXT_LINES as text, i}
  {@const scale = i === 0 ? 1 : 0.5}
  {@const distZ = -0.22 - i * 0.07 - (i > 0 ? 0.07 : 0)}
    <Text 
      position={[1.4, -7.0, distZ]} 
      rotation={[1.57, 0, 0]} 
      scale={scale}
      color={"black"}
      text={text} /> 
  {/each}
  
{/if}