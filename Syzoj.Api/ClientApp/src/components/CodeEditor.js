import React, { Component } from 'react';
import { ListGroup, ListGroupItem, Row, Col } from 'react-bootstrap';

import ace from 'brace';
import 'brace/mode/c_cpp';
import 'brace/mode/pascal';
import 'brace/mode/csharp';
import 'brace/theme/tomorrow';

export class CodeLanguageSelector extends Component {
    constructor(props) {
        super(props);
        this.state = { language: props.defaultLanguage };
    }

    setLanguage(language) {
        this.setState({ language: language});
        this.props.onChange(language);
    }

    render() {
        return <ListGroup>
          <ListGroupItem active={this.state.language === 'cpp'} onClick={(props) => this.setLanguage('cpp')}>C++</ListGroupItem>
          <ListGroupItem active={this.state.language === 'c'} onClick={(props) => this.setLanguage('c')}>C</ListGroupItem>
          <ListGroupItem active={this.state.language === 'pascal'} onClick={(props) => this.setLanguage('pascal')}>Pascal</ListGroupItem>
          <ListGroupItem active={this.state.language === 'csharp'} onClick={(props) => this.setLanguage('csharp')}>C#</ListGroupItem>
        </ListGroup>
    }
}

export class CodeEditor extends Component {
    language_mode = {
        'c' : 'c_cpp',
        'cpp' : 'c_cpp',
        'pascal': 'pascal',
        'csharp': 'csharp',
    }

    onLanguageChange(language)
    {
        this.editor.getSession().setMode("ace/mode/" + this.language_mode[language]);
    }

    render() {
        return <Row>
            <Col xs={4} sm={3}>
                <CodeLanguageSelector language={this.props.language} onChange={(l) => this.onLanguageChange(l)} />
            </Col>
            <Col xs={12} sm={9}>
                <div ref={(editor) => this.refEditor = editor} style={{"border": "1px solid #D4D4D5", "height": "500px"}}></div>
            </Col>
        </Row>;
    }

    componentDidMount()
    {
        this.editor = ace.edit(this.refEditor);
        this.editor.setTheme("ace/theme/tomorrow");
        this.onLanguageChange(this.props.language)
        this.editor.getSession().setUseSoftTabs(false);

        this.editor.container.style.lineHeight = 1.6;
        this.editor.container.style.fontSize = '14px';
        this.editor.container.style.fontFamily = "'Roboto Mono', 'Bitstream Vera Sans Mono', 'Menlo', 'Consolas', 'Lucida Console', monospace";
        this.editor.setShowPrintMargin(false);
        this.editor.renderer.updateFontSize();
    }
}