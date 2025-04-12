// custom version of https://github.com/threlte/threlte/blob/e2b15aabb4c9544894cd333eb3901d92a887603c/packages/extras/src/lib/hooks/useGltfAnimations.ts#L52
// since I don't want to automatically play the animation

import { currentWritable, useTask, watch, type CurrentWritable } from "@threlte/core";
import { tick } from "svelte";
import { derived, writable, type Writable } from "svelte/store";
import { AnimationMixer, Object3D, type AnimationAction } from "three";
import type { GLTF } from "three/examples/jsm/loaders/GLTFLoader.js";

type UseGltfAnimationsReturnType<Actions> = {
    gltf: Writable<GLTF | undefined>
    mixer: AnimationMixer
    actions: CurrentWritable<Actions>
    root: CurrentWritable<Root | undefined>
};

type Root = Object3D;
const isRoot = (value: any): value is Root => !!value?.isObject3D;

type GltfStore = Writable<GLTF | undefined>;
const isGltfStore = (value: any): value is GltfStore =>
  !!value?.subscribe && typeof value.subscribe === 'function';

export function useGltfAnimations<
  T extends string,
  Actions extends Partial<Record<T, AnimationAction>> = Partial<Record<T, AnimationAction>>
>(root?: Root): UseGltfAnimationsReturnType<Actions>
export function useGltfAnimations<
  T extends string,
  Actions extends Partial<Record<T, AnimationAction>> = Partial<Record<T, AnimationAction>>
>(gltf: GltfStore, root?: Root): UseGltfAnimationsReturnType<Actions>

export function useGltfAnimations<
  T extends string,
  Actions extends Partial<Record<T, AnimationAction>> = Partial<Record<T, AnimationAction>>
>(rootOrGltf?: Root | GltfStore, maybeRoot?: Root): UseGltfAnimationsReturnType<Actions> {
  const gltf = isGltfStore(rootOrGltf) ? rootOrGltf : writable<GLTF | undefined>(undefined)
  const root = currentWritable<Root | undefined>(
    isRoot(rootOrGltf) ? rootOrGltf : isRoot(maybeRoot) ? maybeRoot : undefined
  )

  const actualRoot = derived([root, gltf], ([root, gltf]) => {
    return root ?? gltf?.scene
  })

  const actions = currentWritable<Actions>({} as Actions)
  const mixer = new AnimationMixer(undefined as unknown as Object3D)

  watch([gltf, actualRoot], async ([gltf, actualRoot]) => {
    if (!gltf || !gltf.animations.length || !actualRoot) return
    // we need to wait for the tick in order for the ref and
    // its children to be mounted properly
    await tick()
    // if there's a mixer, we stop all running actions
    const newActions = gltf.animations.reduce((acc, clip) => {
      const action = mixer.clipAction(clip, actualRoot)
      return {
        ...acc,
        [clip.name as T]: action
      }
    }, {} as Actions)
    actions.set(newActions)

    return () => {
      Object.values(newActions).forEach((a) => {
        const action = a as AnimationAction
        action.stop()
        mixer.uncacheClip(action.getClip())
      })
    }
  });

  const { start, stop } = useTask(
    (delta) => {
      mixer.update(0)
    },
    { autoStart: false }
  )

  watch(actions, (actions) => {
    if (Object.keys(actions).length) start()
    else stop()
  })

  return {
    gltf,
    root,
    mixer,
    actions
  };
}