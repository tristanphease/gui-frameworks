<script lang="ts">
    import { linear } from "svelte/easing";


    type MenuItem = { url: string, name: string, description: string }; 
    const menuItems: MenuItem[] = [
        { url: ".", name: "Home", description: "The home page" },
        { url: "mathml", name: "MathML", description: "A short dissertation on the Maths Markup Language for the web."},
    ];

    let menuOpened = $state(true);

    function squash(node: HTMLElement, params: { delay?: number, duration?: number, easing?: (t: number) => number }) {
		const existingTransform = getComputedStyle(node).transform.replace('none', '');

		return {
			delay: params.delay || 0,
			duration: params.duration || 400,
			easing: params.easing || linear,
			css: (t: any, u: any) => `transform: ${existingTransform} scaleY(${t})`
		};
	}

</script>


<button id="menu-toggle" aria-label="Toggle Menu" onclick={() => {menuOpened = !menuOpened;document.getElementById("menu-toggle-img")!.offsetWidth}}>
    <img id="menu-toggle-img" alt="Arrow to toggle menu" src="./menu/arrow.svg" style="width: 100%; height: 100%"
        class={menuOpened ? "rotate-anim-forward" : "rotate-anim-backward" }
        />
</button>

<div id="menu">
{#if menuOpened}
    <div id="menu-item-collection" style="transform-origin: top;" transition:squash={{ duration: 200 }}>
    {#each menuItems as menuItem}
        <a class="menu-item" href="/{menuItem.url}">
            <span class="menu-item-name">{menuItem.name}</span>
            <p class="menu-item-description">{menuItem.description}</p>
        </a>
    {/each}
    </div>
{/if}
</div>

<style> 

#menu {
    display: flex;
    flex-direction: row;
    justify-content: center;
}

#menu-item-collection {
    display: flex;
    flex-direction: row;
    justify-content: center;
    gap: 20px;
    padding-left: 20px;
    padding-right: 20px;
}

.menu-item {
    /* flex-grow: 1; */
    text-align: center;
    font-family: 'Franklin Gothic Medium', 'Arial Narrow', Arial, sans-serif;
    text-decoration: none;
}

.menu-item-name {
    font-size: 25px;
}

#menu-toggle {
    height: 50px;
    aspect-ratio: 1;
    position: absolute;
    left: 10px;
    top: 10px;
    z-index: 1;
}

.rotate-anim-forward {
    animation: 0.3s linear 0s forwards arrow-rotate;
}

.rotate-anim-backward {
    animation: 0.3s linear 0s forwards arrow-rotate-back;
}

@keyframes arrow-rotate {
    from {
        transform: rotate(0deg);
    }

    to {
        transform: rotate(90deg);
    }
}

@keyframes arrow-rotate-back {
    from {
        transform: rotate(90deg);
    }

    to {
        transform: rotate(0deg);
    }
}

@media (prefers-color-scheme: dark) {
    #menu-item-collection {
        background-color: rgb(15, 15, 15);
    }

    .menu-item {
        color: #ffffff;
    }

    #menu-toggle {
        background-color: rgb(196, 196, 196);
        border: 2px solid black;
    }
}

</style>