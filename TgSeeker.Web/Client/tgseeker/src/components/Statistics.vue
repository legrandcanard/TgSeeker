<template>
<div class="panel">
	Metrics:
    <ul class="metrics-list" v-if="metrics">
		<li v-for="(value, key) in metrics" :key="key">
			{{ key }}: {{ value }}
		</li>
    </ul>
	<span v-else>...</span>
</div>
</template>

<script>
import { store } from '/src/store';

export default {
    data() {
        return {
            metrics: null
        }
    },
    async mounted() {
        this.metrics = await store.getMetrics();
    }
}
</script>

<style lang="css" scoped>
.panel {
	text-align: initial;
	font-family: 'IBM VGA';
	color: #8c8c8c;
}
.metrics-list {
	list-style: none;
	padding-left: 4px;
}
</style>