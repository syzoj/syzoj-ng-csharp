import React, { Component } from 'react';
import ReactMarkdown from 'react-markdown';
import { Col, Grid, Row, Button } from 'react-bootstrap';
import { CodeEditor } from './CodeEditor';

export class Problem extends Component {
  displayName = Problem.name

  constructor(props) {
    super(props);
    this.state = { loading: true };
  }

  componentDidMount()
  {
    fetch('api/problemset/cd6363d3-ca44-4c81-b660-15db225a91cc/problem/' + this.props.match.params.id)
      .then(response => response.json())
      .then(data => {
        this.setState({ statement: data, loading: false });
      });
  }

  renderMarkdown(name, content)
  {
    if(content != null && content !== "")
    {
      return <div>
        <h2>{name}</h2>
        <ReactMarkdown source={content} />
      </div>;
    }
    else
    {
      return null;
    }
  }

  setLanguage(language)
  {
    this.setState({language: language});
  }

  do_submit()
  {
    alert(this.codeEditor.editor.getValue())
    
  }

  render() {
    return this.state.loading ?
        <p>Loading...</p>
    :
    <Grid>
      <Row>
        <Col xs={12} sm={12}>
          <div className="page-header text-center">
            <h1>#{this.state.statement.ProblemsetProblemId}. {this.state.statement.Title}</h1>
          </div>
        </Col>
      </Row>
      <Row>
        <Col xs={12} sm={12}>
          {this.renderMarkdown("题目描述", this.state.statement.Content.Description)}
          {this.renderMarkdown("输入格式", this.state.statement.Content.InputFormat)}
          {this.renderMarkdown("输出格式", this.state.statement.Content.OutputFormat)}
          {this.renderMarkdown("样例", this.state.statement.Content.Example)}
          {this.renderMarkdown("数据范围与提示", this.state.statement.Content.LimitsAndHints)}
        </Col>
      </Row>
      <CodeEditor language="cpp" ref={(editor) => this.codeEditor = editor} />
      <Button onClick={() => this.do_submit()}>提交</Button>
    </Grid>;
  }
}
