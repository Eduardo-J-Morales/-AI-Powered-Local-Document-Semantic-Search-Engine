<script lang="ts">
	import { onMount } from 'svelte';
	let documents: any[] = [];
	let selectedFile: File | null = null;
	let searchQuery = '';
	let selectedDoc: any = null;
	let tagInput = '';
	let isUploading = false;
	let aiSuggestedTags: string[] = [];
	let userTags: string[] = [];

	onMount(async () => {
		await fetchDocuments();
	});

	async function fetchDocuments() {
		const res = await fetch('/api/documents');
		documents = await res.json();
	}

	async function handleFileChange(event: Event) {
		const files = (event.target as HTMLInputElement).files;
		if (files && files.length > 0) {
			selectedFile = files[0];
		}
	}

	async function uploadDocument() {
		if (!selectedFile) return;
		isUploading = true;
		const formData = new FormData();
		formData.append('file', selectedFile);

		const res = await fetch('/api/documents/upload', {
			method: 'POST',
			body: formData
		});
		isUploading = false;
		selectedFile = null;
		await fetchDocuments();
	}

</script>

<main>
	
</main>

<style>
main {
	max-width: 800px;
	margin: 2rem auto;
	padding: 2rem;
	background: #fff;
	border-radius: 8px;
	box-shadow: 0 2px 8px rgba(0,0,0,0.1);
	font-family: system-ui, sans-serif;
}
section {
	margin-bottom: 2rem;
}
input[type="file"] {
	margin-right: 1rem;
}

button {
	margin-left: 0.5rem;
}
</style>