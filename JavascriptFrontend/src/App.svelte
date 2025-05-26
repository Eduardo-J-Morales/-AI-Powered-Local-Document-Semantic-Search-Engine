<script>
	import { onMount } from 'svelte';
	let documents: any[] = [];
	let selectedFile: File | null = null;
	let searchQuery = '';
	let selectedDoc: any = null;
	let tagInput = '';
	let isUploading = false;
	let aiSuggestedTags: string[] = [];
	let userTags: string[] = [];

	// Fetch all documents on mount
	onMount(async () => {
		await fetchDocuments();
	});

	async function fetchDocuments() {
		const res = await fetch('/api/documents');
		documents = await res.json();
	}


</script>
<main>
	
</main>