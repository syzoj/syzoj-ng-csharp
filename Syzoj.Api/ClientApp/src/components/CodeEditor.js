import React, { Component } from 'react'
import { ListGroup, ListGroupItem, Row, Col } from 'react-bootstrap'

import ace from 'brace'
import 'brace/mode/c_cpp'
import 'brace/mode/pascal'
import 'brace/mode/csharp'
import 'brace/theme/tomorrow'

// The component should be considered uncontrolled
export class CodeEditor extends Component {
    constructor(props) {
        super(props)
        this.state = {lang: this.props.defaultLanguage || 'c'}
        this.languages = this.props.languages || ['c', 'cpp', 'pascal', 'csharp']
    }

    language_table = {
        'c' : {mode: 'ace/mode/c_cpp', caption: 'C'},
        'cpp' : {mode: 'ace/mode/c_cpp', caption: 'C++'},
        'pascal': {mode: 'ace/mode/pascal', caption: 'Pascal'},
        'csharp': {mode: 'ace/mode/csharp', caption: 'C#'}
    }

    setLang(lang) {
        this.setState({lang: lang})
    }

    getData() {
        return {
            lang: this.state.lang,
            value: this.editor.getValue()
        }
    }

    render() {
        return <Row>
            <Col xs={4} sm={3}>
                <ListGroup>
                    {this.languages.map(lang => 
                        <ListGroupItem key={lang} active={this.state.lang === lang} onClick={(props) => this.setLang(lang)}>{this.language_table[lang].caption}</ListGroupItem>
                    )}
                </ListGroup>
            </Col>
            <Col xs={12} sm={9}>
                <div ref={(editor) => this.refEditor = editor} style={{"border": "1px solid #D4D4D5", "height": "500px"}}></div>
            </Col>
        </Row>;
    }

    componentDidMount()
    {
        this.editor = ace.edit(this.refEditor)
        this.editor.setTheme("ace/theme/tomorrow")
        this.editor.getSession().setMode(this.language_table[this.state.lang].mode)
        this.editor.getSession().setUseSoftTabs(false)

        this.editor.container.style.lineHeight = 1.6
        this.editor.container.style.fontSize = '14px'
        this.editor.container.style.fontFamily = "'Roboto Mono', 'Bitstream Vera Sans Mono', 'Menlo', 'Consolas', 'Lucida Console', monospace"
        this.editor.setShowPrintMargin(false)
        this.editor.renderer.updateFontSize()

        if(this.props.defaultValue) {
            this.editor.setValue(this.props.defaultValue, -1)
        }
    }

    componentDidUpdate(prevProps, prevState) {
        if(this.state.lang !== prevState.lang) {
            this.editor.getSession().setMode(this.language_table[this.state.lang].mode)
        }
    }
}