<script lang="ts">
  import Menu from "./../lib/menu.svelte";
  let { children } = $props();
</script>

<Menu />
{@render children()}