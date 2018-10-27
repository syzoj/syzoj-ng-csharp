export default {
    api: "",
    problemTypes: [
        {id: "standard", title: "标准类型", createComponent: () => import('./components/problem/standard/Create').then(v => v.default)}
    ],
    problemType: {
        "standard": {}
    },
    templates: {
        "problem-standard-view": () => import('./components/ViewModel/problem-standard-view').then(v => v.default),
        "problem-standard-submission": () => import('./components/ViewModel/problem-standard-submission').then(v => v.default)
    },
    defaultProblemsetId: "8d66b710-e803-4f3e-a94c-eabcd9683c67"
}