import React, { Component } from 'react'
import { CodeEditor } from '../CodeEditor'
import Button from 'react-bootstrap/lib/Button'
import { request } from '../../util'

export default class View extends Component {
    submit() {
        var data = this.refEditor.getData()
        request(`/api/problem-standard/${this.props.content.problemId}/submit`, 'POST', {
            Language: data.lang,
            Code: data.value
        }).then(response => this.props.onSubmit(response.Token))
    }

    render() {
        return <div>
            <p>`This is standard problem, data: ${JSON.stringify(this.props.content)}`</p>
            <CodeEditor ref={(editor) => this.refEditor = editor} languages={['c', 'cpp', 'pascal', 'csharp']} />
            <Button onClick={() => this.submit()}>Submit</Button>
        </div>
    }
}